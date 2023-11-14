using bthrm.core.Data.Model;

namespace bthrm.core.Websockets.Messages;

public class GetSessionRequest : BaseRequestPacket<int?>
{
    public override int Type => (int)RequestPacketType.GetSession;
}