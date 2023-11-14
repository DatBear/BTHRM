using bthrm.core.Data.Model;

namespace bthrm.core.Websockets.Messages;

public class SetUserResponse : BaseResponsePacket<User>
{
    public override int Type => (int)ResponsePacketType.SetUser;
}