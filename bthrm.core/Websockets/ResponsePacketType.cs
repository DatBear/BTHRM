namespace bthrm.core.Websockets;

public enum ResponsePacketType
{
    Ping,
    Pong,
    GetUser,
    SetUser,
    StartSession,
    GetSession,
    StopSession,
    GetHeartRate,

    Error = 255,
}