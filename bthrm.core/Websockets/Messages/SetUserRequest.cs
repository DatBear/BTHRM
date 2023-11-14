namespace bthrm.core.Websockets.Messages;

public class SetUserRequest : BaseRequestPacket<int>
{
    public override int Type => (int)RequestPacketType.SetUser;
}