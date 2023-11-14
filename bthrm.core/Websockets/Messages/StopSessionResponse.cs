using bthrm.core.Data.Model;

namespace bthrm.core.Websockets.Messages;

public class StopSessionResponse : BaseResponsePacket<HeartRateSession>
{
    public override int Type => (int)ResponsePacketType.StopSession;
}