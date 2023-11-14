import type HeartRateReading from "./HeartRateReading";

type HeartRateSession = {
  id: number;
  userId: number;
  startDate: string;
  endDate?: string;
  isPaused: boolean;
  readings: HeartRateReading[];
}

export default HeartRateSession;