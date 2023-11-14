import { useEffect, useState } from "react";
import GraphHeartRateReading from "~/models/GraphHeartRateReading";
import HeartRateReading from "~/models/HeartRateReading";
import RequestPacketType from "~/network/RequestPacketType";
import ResponsePacketType from "~/network/ResponsePacketType";
import socket, { listen, send } from "~/network/Socket";

export default function Home() {
  const [data, setData] = useState<GraphHeartRateReading[]>([]);

  useEffect(() => {
    socket().onopen = () => {
      send(RequestPacketType.SetUser, 1, true);
    }
  }, []);

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
    home
  </>
}

