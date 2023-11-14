using NetCoreServer;
using Newtonsoft.Json;
using System.Text;
using bthrm.core.Websockets;
using Microsoft.Extensions.DependencyInjection;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Reflection;
using bthrm.server.Extensions;
using bthrm.server.Websockets.Handlers;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using MediatR;
using Nito.AsyncEx;

namespace bthrm.server.Websockets;

public class HeartRateSession : WsSession
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfigurationRoot _config;

    public Guid Id { get; set; } = Guid.NewGuid();
    public int? UserId { get; set; }

    public HeartRateSession(WsServer server, IServiceProvider serviceProvider, IConfigurationRoot config) : base(server)
    {
        _serviceProvider = serviceProvider;
        _config = config;
    }

    protected override void OnConnected()
    {
        var sessionManager = _serviceProvider.GetService<HeartRateSessionManager>()!;
        sessionManager.AddSession(this);
        base.OnConnected();
    }

    protected override void OnDisconnected()
    {
        var sessionManager = _serviceProvider.GetService<HeartRateSessionManager>()!;
        sessionManager.RemoveSession(this);
        base.OnDisconnected();
    }

    public override void OnWsReceived(byte[] buffer, long offset, long size)
    {
        var message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        var nullPacket = JsonConvert.DeserializeObject<NullPacket>(message);

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var serviceCollection = CreateServiceCollection();
        var provider = serviceCollection.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        var mapper = provider.GetRequiredService<PacketMapper>();
        var packet = mapper.Deserialize((RequestPacketType)nullPacket.Type, message);

        if (packet != null)
        {
            try
            {
                AsyncContext.Run(async () => await mediator.Send(packet));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine($"time: {stopwatch.ElapsedMilliseconds}ms");
        }
    }

    public bool Send<T>(T? obj) where T : IResponsePacket
    {
        if (obj == null) return false;
        return SendTextAsync(JsonConvert.SerializeObject(obj));
    }

    protected bool Equals(HeartRateSession other)
    {
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((HeartRateSession)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    private ServiceCollection CreateServiceCollection()
    {
        var collection = new ServiceCollection();
        collection.AddSharedServices(_config);

        collection.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(SetUserHandler).Assembly));
        collection.AddSingleton(this);
        Assembly.GetAssembly(typeof(IRequestPacket))!.GetTypesAssignableFrom<IRequestPacket>()
                .ForEach(x => collection.AddTransient(typeof(IRequestPacket), x));
        collection.AddTransient<PacketMapper>();

        collection.AddSingleton(_serviceProvider.GetService<HeartRateSessionManager>()!);

        return collection;
    }
}