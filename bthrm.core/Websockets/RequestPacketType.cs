namespace bthrm.core.Websockets;

public enum RequestPacketType
{
    Ping,
    Pong,
    GetUser,
    SetUser,
    StartSession,
    GetSession,
    StopSession,
    AddHeartRateReading,
    GetHeartRate,
}