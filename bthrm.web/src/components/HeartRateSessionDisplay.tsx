import { useEffect, useState } from "react";
import GraphHeartRateReading, { toGraphData } from "~/models/GraphHeartRateReading";
import HeartRateSession from "~/models/HeartRateSession";
import HeartRateGraph from "./HeartRateGraph";
import { listen } from "~/network/Socket";
import ResponsePacketType from "~/network/ResponsePacketType";
import type HeartRateReading from "~/models/HeartRateReading";
import { minutes, descriptiveTimeDiff, timeDiff, timeString } from "~/utils/date";
import type User from "~/models/User";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faClock, faFire, faHeart, faStopwatch } from "@fortawesome/free-solid-svg-icons";
import Section from "./Section";

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

  if (!session) {
    return <></>
  }

  const startDate = new Date(session.startDate);
  const endDate = session.endDate ? new Date(session.endDate) : new Date();

  const averageHeartRate = () => {
    if (!session?.readings?.length) return 0;
    return session.readings.filter(x => x != null).map(x => x.heartRate).reduce((p, c) => p + c, 0) / session.readings.length
  }

  const maxHeartRate = () => {
    if (!session?.readings?.length) return 0;
    return session.readings.filter(x => x != null).map(x => x.heartRate).reduce((p, c) => c > p ? c : p, 0)
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

  const dateRange = () => {
    if (!session.endDate) return startDate.toLocaleDateString();
    const isSingleDay = startDate.getDate() === endDate.getDate();
    return isSingleDay ? new Date(session.startDate).toLocaleDateString() : `${startDate.toLocaleDateString()} - ${endDate.toLocaleDateString()}`;
  }



  return <div className="w-full p-10 flex flex-col items-center">
    {session.startDate && <div className="text-2xl">
      {dateRange()}
    </div>}
    <div className="text-xl">
      {new Intl.DateTimeFormat("en-US", { weekday: "long" }).format(startDate)}, {timeString(startDate)}
    </div>
    <Section icon={faStopwatch} iconColor="text-green-700" name="" value={timeDiff(startDate, endDate)} />

    <div className="grid grid-cols-2 justify-items-center w-full">
      {!session.endDate && <Section icon={faHeart} iconColor="text-red-600" name="bpm" value={session?.readings[(session?.readings?.length ?? 1) - 1]?.heartRate ?? 0} />}
      {!session.endDate && <div>{/*figure out what to put here...*/}</div>}
      <Section icon={faHeart} iconColor="text-red-600" name="avg bpm" value={Math.round(averageHeartRate())} />
      <Section icon={faHeart} iconColor="text-red-600" name="max bpm" value={maxHeartRate()} />

      <Section icon={faFire} iconColor="text-orange-600" name="cals" value={Math.round(caloriesBurned())} />
      <Section icon={faFire} iconColor="text-orange-600" name="cals" value={Math.round(caloriesBurned() / minutes(startDate, endDate) * 10) / 10} />
    </div>

    <div className="flex flex-col items-center mt-10 w-full -ml-10">
      <HeartRateGraph data={graphData} />
    </div>
  </div>
}