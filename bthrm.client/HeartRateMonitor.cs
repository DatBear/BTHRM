using System.Collections.Specialized;
using InTheHand.Bluetooth;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Xml.Linq;

namespace bthrm;

public class HeartRateMonitor
{
    private static readonly BluetoothUuid HeartRateServiceUuid = BluetoothUuid.FromShortId(0x180D);
    private static readonly BluetoothUuid HeartRateMeasurementCharacteristicUuid = BluetoothUuid.FromShortId(0x2A37);

    private readonly string _name;
    private BluetoothDevice? _device;
    private GattCharacteristic? _characteristic;
    private SortedList<DateTime, int> _readings = new();

    public int HeartRate { get; set; }
    public event EventHandler<HeartRateUpdated> HeartRateUpdated;
    

    public HeartRateMonitor(string name)
    {
        _name = name;
    }

    public async Task<bool> StartListening(int timeout = 5000)
    {
        _readings = new();

        var device = _device ?? await Scan(timeout);

        if (device == null)
        {
            throw new Exception($"Could not find device {_name}");
        }

        if (_characteristic == null)
        {
            var service = await device.Gatt.GetPrimaryServiceAsync(HeartRateServiceUuid);
            _characteristic = await service.GetCharacteristicAsync(HeartRateMeasurementCharacteristicUuid);
        }

        if (_characteristic == null)
        {
            throw new Exception("Could not find characteristic");
        }

        if ((_characteristic.Properties & GattCharacteristicProperties.Notify) == GattCharacteristicProperties.Notify)
        {
            _characteristic.CharacteristicValueChanged -= Characteristic_ValueChanged;
            _characteristic.CharacteristicValueChanged += Characteristic_ValueChanged;
            await _characteristic.StartNotificationsAsync();
        }
        return false;
    }

    private void Characteristic_ValueChanged(object? sender, GattCharacteristicValueChangedEventArgs e)
    {
        SetHeartRate(e.Value[1]);
        //Console.WriteLine($"Heart rate char value: {string.Join(" ", e.Value.Select(x => x.ToString("X2")))}");
    }

    public async Task StopListening()
    {
        if (_characteristic == null)
        {
            return;
        }

        _characteristic.CharacteristicValueChanged -= Characteristic_ValueChanged;
    }

    public async Task<BluetoothDevice?> Scan(int timeout = 5000)
    {
        CancellationTokenSource timeoutCts = new CancellationTokenSource();
        timeoutCts.CancelAfter(timeout);

        CancellationTokenSource cts = new CancellationTokenSource();
        var handler = Bluetooth_AdvertisementReceived(cts);
        Bluetooth.AdvertisementReceived += handler;

        var bluetoothScan = Bluetooth.ScanForDevicesAsync(new RequestDeviceOptions
        {
            Filters =
            {
                new BluetoothLEScanFilter()
                {
                    NamePrefix = "Polar"
                }
            }
        });

        var le = await Bluetooth.RequestLEScanAsync(new BluetoothLEScanOptions
        {
            KeepRepeatedDevices = false,
            AcceptAllAdvertisements = false,
            Filters =
            {
                new BluetoothLEScanFilter
                {
                    Services = { HeartRateServiceUuid },
                    NamePrefix = _name
                }
            }
        });
        

        while (!cts.IsCancellationRequested && !timeoutCts.IsCancellationRequested)
        {
            await Task.Delay(100);
        }

        Bluetooth.AdvertisementReceived -= handler;
        le.Stop();

        _device ??= (await bluetoothScan).FirstOrDefault();
        return _device;
    }
    

    private EventHandler<BluetoothAdvertisingEvent> Bluetooth_AdvertisementReceived(CancellationTokenSource cts)
    {
        return (sender, e) =>
        {
            if (string.IsNullOrEmpty(e.Name))
            {
                return;
            }

            Console.WriteLine($"Found {e.Name}");

            if (e.Name.StartsWith(_name))
            {
                var success = false;   
                Task.Run(async () =>
                {
                    
                    var service = await e.Device.Gatt.GetPrimaryServiceAsync(HeartRateServiceUuid);
                    if(service != null)
                    {
                        var characteristic = await service.GetCharacteristicAsync(HeartRateMeasurementCharacteristicUuid);
                        if (characteristic != null)
                        {
                            _device = e.Device;
                            _characteristic = characteristic;
                            success = true;
                        }
                    }
                }).GetAwaiter().GetResult();
                if (success)
                {
                    cts.Cancel();
                }
            }
        };
    }

    private void SetHeartRate(int rate)
    {
        HeartRate = rate;
        HeartRateUpdated?.Invoke(this, new HeartRateUpdated(HeartRate));

        if (rate <= 0)
        {
            return;
        }

        _readings.Add(DateTime.UtcNow, rate);
    }

    public double GetAverage(int seconds = 3)
    {
        var readingsAfter = DateTime.UtcNow.AddSeconds(-seconds);
        if (!_readings.Any()) return 0;
        var readings = _readings.Reverse().TakeWhile(x => x.Key >= readingsAfter).ToList();
        if (!readings.Any()) return 0;
        return readings.Average(x => x.Value);
    }
}