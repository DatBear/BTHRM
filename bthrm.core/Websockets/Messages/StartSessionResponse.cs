using bthrm.core.Data.Model;

namespace bthrm.core.Websockets.Messages;

public class StartSessionResponse : BaseResponsePacket<HeartRateSession>
{
    public override int Type => (int)ResponsePacketType.StartSession;
}