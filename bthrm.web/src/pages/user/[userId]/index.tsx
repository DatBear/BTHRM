import Link from "next/link";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import GraphHeartRateReading from "~/models/GraphHeartRateReading";
import HeartRateReading from "~/models/HeartRateReading";
import RequestPacketType from "~/network/RequestPacketType";
import ResponsePacketType from "~/network/ResponsePacketType";
import socket, { listen, send } from "~/network/Socket";

export default function User() {
  const router = useRouter();
  const { userId } = router.query;
  const [data, setData] = useState<GraphHeartRateReading[]>([]);

  useEffect(() => {
    socket().onopen = () => {
      send(RequestPacketType.SetUser, userId, true);
    }
  }, [userId]);

  useEffect(() => {
    return listen(ResponsePacketType.GetHeartRate, (e: HeartRateReading) => {
      const graphData = {
        ...e,
        time: new Date(e.date).getTime()
      };

      setData(x => [...x, graphData]);
      console.log(graphData);
    });
  }, []);



  return <>
    Home
    <Link href="/user/1">User 1</Link>
  </>
}
