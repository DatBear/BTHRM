using bthrm.core.Data.Model;

namespace bthrm.core.Websockets.Messages;

public class StartSessionRequest : BaseRequestPacket<HeartRateSession>
{
    public override int Type => (int)RequestPacketType.StartSession;
}