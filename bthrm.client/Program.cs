using InTheHand.Bluetooth;
using System.Net.WebSockets;
using System.Text;
using bthrm.core.Websockets.Messages;
using dotenv.net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Windows.Networking.Sockets;
using bthrm.core.Data.Model;
using Nito.AsyncEx;

namespace bthrm
{
    internal class Program
    {
        private static HeartRateWebsocket _websocket;
        private static Random _r = new Random();

        static async Task Main(string[] args)
        {
            DotEnv.Load(new DotEnvOptions(true, probeForEnv: true, probeLevelsToSearch: 5));
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            var websocketConfig = configuration.GetSection("Websocket").Get<WebsocketConfig>();

            await StartHeartRateMonitor(false);

            _websocket = new HeartRateWebsocket(websocketConfig!);
            await _websocket.Connect();
            await _websocket.Send(new SetUserRequest
            {
                Data = 1
            });

            while (true)
            {
                await Task.Delay(1000);
            }
        }

        private static async Task StartHeartRateMonitor(bool isFake)
        {
            if (isFake)
            {
                await Task.Run(FakeHeartRateThread);
            }

            var device = new HeartRateMonitor("Polar");
            device.HeartRateUpdated += Device_HeartRateUpdated;
            await device.Scan();
            await device.StartListening();
        }

        private static int FakeHeartRate = 80;
        private static async Task FakeHeartRateThread()
        {
            while (true)
            {
                FakeHeartRate += _r.Next(FakeHeartRate > 160 ? -2 : -1, FakeHeartRate < 140 ? 2 : 1);
                await _websocket.Send(new AddHeartRateReadingRequest
                {
                    Data = new HeartRateReading
                    {
                        Date = DateTime.UtcNow,
                        HeartRate = FakeHeartRate
                    }
                });

                await Task.Delay(500);
            }
            
        }

        private static void Device_HeartRateUpdated(object? sender, HeartRateUpdated e)
        {
            try
            {
                AsyncContext.Run(async () =>
                {
                    if (_websocket == null) throw new Exception("websocket not available");
                    await _websocket.Send(new AddHeartRateReadingRequest
                    {
                        Data = new HeartRateReading
                        {
                            Date = DateTime.UtcNow,
                            HeartRate = e.HeartRate
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending heart rate", ex);
            }
            Console.WriteLine($"Heart rate: {e.HeartRate}");
            if (sender is HeartRateMonitor monitor)
            {
                Console.WriteLine($"\tAvg: {monitor.GetAverage()}");
            }
        }
    }
}