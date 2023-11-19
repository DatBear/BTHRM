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

import { Roboto } from '@next/font/google';
import clsx from "clsx";

const roboto = Roboto({
  subsets: ['latin'],
  weight: '400'
});

type HeartRateSessionProps = {
  initialSession?: HeartRateSession;
  user?: User;
}

export default function HeartRateSessionDisplay({ initialSession, user }: HeartRateSessionProps) {
  const [session, setSession] = useState<HeartRateSession | undefined>(initialSession);
  const [graphData, setGraphData] = useState<GraphHeartRateReading[]>([]);

  useEffect(() => {
    if (!initialSession) return;
    setSession(initialSession);
    setGraphData(initialSession.readings?.filter(x => x).map(x => toGraphData(x)));
  }, [initialSession]);

  useEffect(() => {
    return listen(ResponsePacketType.GetHeartRate, (e: HeartRateReading) => {
      if (!e) {
        return;
      }
      const data = toGraphData(e);
      //console.log('array', graphData);
      //console.log('data', data);
      setSession(x => ({ ...x!, readings: [...x!.readings, e] }));
      setGraphData([...graphData, data]);
    });
  }, [graphData]);

  if (!initialSession) {
    return <></>
  }

  const startDate = new Date(initialSession.startDate);
  const endDate = initialSession.endDate ? new Date(initialSession.endDate) : new Date();

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
    if (!session?.endDate) return startDate.toLocaleDateString();
    const isSingleDay = startDate.getDate() === endDate.getDate();
    return isSingleDay ? new Date(initialSession.startDate).toLocaleDateString() : `${startDate.toLocaleDateString()} - ${endDate.toLocaleDateString()}`;
  }

  if (!session) {
    return <>Session not found</>
  }

  return <div className={clsx(roboto.className, "w-full p-10 flex flex-col items-center gap-y-1")}>
    {session.startDate && <div className="text-2xl">
      {dateRange()}
    </div>}
    <div className="text-xl">
      {new Intl.DateTimeFormat("en-US", { weekday: "long" }).format(startDate)}, {timeString(startDate)}
    </div>
    <Section icon={faStopwatch} iconColor="text-green-700" name="" value={timeDiff(startDate, endDate)} />

    <div className="grid grid-cols-2 justify-items-center w-full gap-y-10 py-10">
      {!session.endDate && <Section icon={faHeart} iconColor="text-red-600" name="bpm" value={session?.readings[(session?.readings?.length ?? 1) - 1]?.heartRate ?? 0} />}
      {!session.endDate && <div>{/*figure out what to put here...*/}</div>}
      <Section icon={faHeart} iconColor="text-red-600" name="avg bpm" value={Math.round(averageHeartRate())} />
      <Section icon={faHeart} iconColor="text-red-600" name="max bpm" value={maxHeartRate()} />

      <Section icon={faFire} iconColor="text-orange-600" name="cals" value={Math.round(caloriesBurned())} />
      <Section icon={faFire} iconColor="text-orange-600" name="avg cpm" value={Math.round(caloriesBurned() / minutes(startDate, endDate) * 10) / 10} />
    </div>

    <div className="flex flex-col items-center mt-10 w-full -ml-10">
      <HeartRateGraph data={graphData} />
    </div>
  </div>
}