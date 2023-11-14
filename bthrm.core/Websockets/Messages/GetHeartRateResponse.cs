using bthrm.core.Data.Model;

namespace bthrm.core.Websockets.Messages;

public class GetHeartRateResponse : BaseResponsePacket<HeartRateReading>
{
    public override int Type => (int)ResponsePacketType.GetHeartRate;
}