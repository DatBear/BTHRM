using bthrm.core.Data.Model;

namespace bthrm.core.Websockets.Messages;

public class GetUserRequest: BaseRequestPacket<int>
{
    public override int Type => (int)RequestPacketType.GetUser;
}

public class GetUserResponse : BaseResponsePacket<User?>
{
    public override int Type => (int)ResponsePacketType.GetUser;
}