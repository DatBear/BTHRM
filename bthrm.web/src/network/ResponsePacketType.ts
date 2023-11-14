enum ResponsePacketType {
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

export default ResponsePacketType;