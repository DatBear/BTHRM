using Newtonsoft.Json;

namespace bthrm.core.Websockets;

public class PacketMapper
{
    private Dictionary<int, Type> _requestTypes;

    public PacketMapper(IEnumerable<IRequestPacket> requestPackets)
    {
        _requestTypes = requestPackets.ToDictionary(x => x.Type, x => x.GetType());
    }

    public object? Deserialize(RequestPacketType type, string message)
    {
        if(_requestTypes.TryGetValue((int)type, out var dType))
        {
            return JsonConvert.DeserializeObject(message, dType);
        }
        return default;
    }
}