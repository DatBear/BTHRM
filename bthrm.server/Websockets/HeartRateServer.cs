using System.Net;
using Microsoft.Extensions.Configuration;
using NetCoreServer;

namespace bthrm.server.Websockets;

public class HeartRateServer : WsServer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfigurationRoot _config;

    public HeartRateServer(IPAddress address, int port, IServiceProvider serviceProvider, IConfigurationRoot config) : base(address, port)
    {
        _serviceProvider = serviceProvider;
        _config = config;
    }

    protected override TcpSession CreateSession()
    {
        return new HeartRateSession(this, _serviceProvider, _config);
    }
}