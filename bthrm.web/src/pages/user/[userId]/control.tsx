import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import HeartRateSession from "~/models/HeartRateSession";
import User from "~/models/User";
import RequestPacketType from "~/network/RequestPacketType";
import ResponsePacketType from "~/network/ResponsePacketType";
import socket, { listen, send } from "~/network/Socket";

export default function Home() {
  const router = useRouter();
  const { userId } = router.query;

  const [user, setUser] = useState<User>();

  useEffect(() => {
    (async () => {
      await send(RequestPacketType.SetUser, userId);
    })();
  }, [userId]);

  useEffect(() => {
    return listen(ResponsePacketType.SetUser, (e: User) => {
      setUser(e);
    }, true);
  }, [userId]);

  useEffect(() => {
    if (!userId) return;
    return listen(ResponsePacketType.GetSession, (e: HeartRateSession) => {
      const url = `/user/${userId}/session/${e.id}/control`;
      console.log('url', url);
      location.href = url;
    }, true);
  }, [userId]);

  const startSession = () => {
    send(RequestPacketType.StartSession, { startDate: new Date() })
  }

  return <div className="flex flex-col">
    Control user {user?.id}
    <button onClick={startSession} className="w-max">Start Session</button>
  </div>
}