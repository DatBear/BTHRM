namespace bthrm.core.Websockets.Messages;

public class StopSessionRequest : BaseRequestPacket<int>
{
    public override int Type => (int)RequestPacketType.StopSession;
}