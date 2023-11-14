enum RequestPacketType {
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

export default RequestPacketType;