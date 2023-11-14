import { useEffect, useState } from "react";
import GraphHeartRateReading, { toGraphData } from "~/models/GraphHeartRateReading";
import HeartRateSession from "~/models/HeartRateSession";
import HeartRateGraph from "./HeartRateGraph";
import { listen } from "~/network/Socket";
import ResponsePacketType from "~/network/ResponsePacketType";
import type HeartRateReading from "~/models/HeartRateReading";
import { minutes, timeDiff } from "~/utils/date";
import type User from "~/models/User";

type HeartRateSessionProps = {
  session?: HeartRateSession;
  user?: User;
}

export default function HeartRateSessionDisplay({ session, user }: HeartRateSessionProps) {
  const [graphData, setGraphData] = useState<GraphHeartRateReading[]>([]);

  useEffect(() => {
    if (!session) return;
    setGraphData(session.readings.map(x => toGraphData(x)));
  }, [session]);

  useEffect(() => {
    return listen(ResponsePacketType.GetHeartRate, (e: HeartRateReading) => {
      const data = toGraphData(e);
      console.log('array', graphData);
      console.log('data', data);
      setGraphData([...graphData, data]);
    });
  }, [graphData]);

  const averageHeartRate = () => {
    if (!session?.readings?.length) return 0;
    return session.readings.filter(x => x != null).map(x => x.heartRate).reduce((p, c) => p + c, 0) / session.readings.length
  }

  const caloriesBurned = () => {
    /*
    Women: CB = T × (0.4472×H - 0.1263×W + 0.074×A - 20.4022) / 4.184
    Men:   CB = T × (0.6309×H + 0.1988×W + 0.2017×A - 55.0969) / 4.184
    */
    if (!session || !user) return 0;
    const t = minutes(new Date(session.startDate), new Date(session.endDate ?? new Date()));
    const h = averageHeartRate();
    const w = user.weight;
    const a = user.age;

    //return t * (.4472 * h - .1263 * w + .074 * a - 20.4022) / 4.184;
    return t * (.6309 * h + .1988 * w + .2017 * a - 55.0969) / 4.184;
  }

  if (!session) {
    return <></>
  }

  return <div className="w-full m-3 p-3">
    {session.startDate && <div>
      Start: {new Date(session.startDate).toLocaleString()}
    </div>}
    {session.endDate && <div>
      End: {new Date(session.endDate).toLocaleString()}
    </div>}
    <div>
      Duration: {timeDiff(new Date(session.startDate), new Date(session.endDate ?? new Date()))}
    </div>
    <div className="flex flex-col items-center">
      <HeartRateGraph data={graphData} />
    </div>

    {!session.endDate && <div>
      BPM: {session.readings[session.readings.length - 1]?.heartRate} bpm
    </div>}
    <div>
      Avg BPM: {Math.round(averageHeartRate())} bpm
    </div>
    <div>Calories burned: {Math.round(caloriesBurned())}</div>
  </div>
}