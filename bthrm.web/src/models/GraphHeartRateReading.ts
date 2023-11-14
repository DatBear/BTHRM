import type HeartRateReading from "./HeartRateReading";

type GraphHeartRateReading = HeartRateReading & {
  time: number;
}

export const toGraphData = (r: HeartRateReading) => {
  return {
    ...r,
    time: new Date(r.date).getTime()
  };
}

export default GraphHeartRateReading;