namespace bthrm.core.Websockets;

public interface IResponsePacket
{
}

public abstract class BaseResponsePacket<T> : BasePacket<T>, IResponsePacket
{
}