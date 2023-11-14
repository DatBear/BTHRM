using MediatR;

namespace bthrm.core.Websockets;

public interface IRequestPacket
{
    int Type { get; }
}

public abstract class BaseRequestPacket<T> : BasePacket<T>, IRequestPacket, IRequest
{
}