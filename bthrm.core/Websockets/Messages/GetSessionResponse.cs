using bthrm.core.Data.Model;

namespace bthrm.core.Websockets.Messages;

public class GetSessionResponse : BaseResponsePacket<HeartRateSession>
{
    public override int Type => (int)ResponsePacketType.GetSession;
}