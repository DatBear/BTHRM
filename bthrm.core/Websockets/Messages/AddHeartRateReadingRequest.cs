using bthrm.core.Data.Model;

namespace bthrm.core.Websockets.Messages;

public class AddHeartRateReadingRequest : BaseRequestPacket<HeartRateReading>
{
    public override int Type => (int)RequestPacketType.AddHeartRateReading;
}