using bthrm.core.Websockets.Messages;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using Windows.Media.Protection.PlayReady;
using bthrm.core.Websockets;

namespace bthrm;

public class HeartRateWebsocket : IDisposable
{
    private readonly WebsocketConfig _config;
    private readonly ClientWebSocket _client;

    public HeartRateWebsocket(WebsocketConfig config)
    {
        _config = config;
        _client = new ClientWebSocket();
    }

    public async Task Connect()
    {
        await _client.ConnectAsync(new Uri(_config.Url), CancellationToken.None);
    }

    public async Task Send<T>(T request) where T : IRequestPacket
    {
        var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
        await _client.SendAsync(data, WebSocketMessageType.Binary, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}