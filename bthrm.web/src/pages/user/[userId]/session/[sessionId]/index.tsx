import { useRouter } from "next/router";
import { useState, useEffect } from "react";
import HeartRateSessionDisplay from "~/components/HeartRateSessionDisplay";
import HeartRateSession from "~/models/HeartRateSession";
import User from "~/models/User";
import RequestPacketType from "~/network/RequestPacketType";
import ResponsePacketType from "~/network/ResponsePacketType";
import socket, { listen, send } from "~/network/Socket";

export default function Home() {
  const router = useRouter();
  const { userId, sessionId } = router.query;
  const [user, setUser] = useState<User>();
  const [session, setSession] = useState<HeartRateSession>();

  useEffect(() => {
    socket().onopen = () => {
      send(RequestPacketType.GetUser, userId);
      send(RequestPacketType.GetSession, sessionId);
    }
  }, [sessionId, userId]);

  useEffect(() => {
    return listen(ResponsePacketType.GetUser, (e: User) => {
      setUser(e);
    }, true);
  }, [userId]);

  useEffect(() => {
    return listen(ResponsePacketType.GetSession, (e: HeartRateSession) => {
      setSession(e);
    }, true);
  }, [sessionId]);


  return <>
    {user && session && <HeartRateSessionDisplay user={user} session={session} />}
  </>
}
