namespace bthrm.core.Websockets;

public interface IPacket
{
}

public abstract class BasePacket<T> : IPacket
{
    public T Data { get; set; }
    public abstract int Type { get; }
}