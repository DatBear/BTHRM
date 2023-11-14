namespace bthrm.core.Websockets.Messages;

public class GetHeartRateRequest : BaseRequestPacket<NullData>
{
    public override int Type => (int)RequestPacketType.GetHeartRate;
}