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
    (async () => {
      await send(RequestPacketType.GetUser, userId);
      await send(RequestPacketType.GetSession, sessionId, true);
    })();
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

  const stopSession = () => {
    if (!session) return;
    send(RequestPacketType.StopSession, session.id);
  }

  const pauseSession = () => {
    if (!session) return;
    //send(RequestPacketType.StopSession, session.id);
  }

  const resumeSession = () => {
    if (!session) return;
    //send(RequestPacketType.StopSession, session.id);
  }

  useEffect(() => {
    return listen(ResponsePacketType.StopSession, (e: HeartRateSession) => {
      var url = `/user/${userId}/session/${e.id}`;
      location.href = url;
    }, true);
  }, [userId]);


  return <>
    <div className="border border-red-500 flex">
      <HeartRateSessionDisplay initialSession={session} user={user} />
    </div>
    <div className="flex flex-col gap-2 p-3">
      {!session?.endDate && <>
        Controls
        <div className="flex flex-row gap-3">
          <button onClick={stopSession}>Stop</button>
          {!session?.isPaused && <button onClick={pauseSession}>Pause</button>}
          {session?.isPaused && <button onClick={resumeSession}>Resume</button>}
        </div>
      </>}
    </div>
  </>
}
