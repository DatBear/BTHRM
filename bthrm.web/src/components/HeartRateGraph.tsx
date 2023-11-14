import { CartesianGrid, Line, LineChart, ResponsiveContainer, Tooltip, TooltipProps, XAxis, YAxis } from "recharts";
import type GraphHeartRateReading from "~/models/GraphHeartRateReading";

type HeartRateGraphProps = {
  data: GraphHeartRateReading[];
}

export default function HeartRateGraph({ data }: HeartRateGraphProps) {
  return <>
    {data && data.length > 0 && <>
      <div className="text-xl">Heart Rate</div>
      <ResponsiveContainer width="95%" height={300}>
        <LineChart data={data}>
          <defs>
            <linearGradient id="colorUv" x1="0%" y1="0%" x2="0%" y2="100%">
              <stop offset="10%" stopColor="red" />
              <stop offset="30%" stopColor="orange" />
              <stop offset="60%" stopColor="yellow" />
              <stop offset="100%" stopColor="green" />
            </linearGradient>
          </defs>
          <Line type="monotone" dataKey="heartRate" stroke="url(#colorUv)" isAnimationActive={false} dot={false} />
          <XAxis
            interval={"preserveStart"} dataKey="time" scale={"time"} type={"number"}
            domain={[data[0]!.time, data[data.length - 1]!.time]}
            tickFormatter={(v => formatDate(v as string))}
            tickLine={false} height={1}
          />
          <YAxis domain={['dataMin', 'dataMax']} width={30} />
          <Tooltip content={<CustomTooltip />} />
        </LineChart>
      </ResponsiveContainer>
    </>}
  </>
}

const CustomTooltip = ({ active, payload, label }: TooltipProps<number, number>) => {
  return active && payload?.length ? <div className="custom-tooltip bg-black p-3 rounded-sm border border-white">
    <div className="label">Time: {`${new Date(label).toLocaleString()}`}</div>
    <div className="value">Heart Rate: {payload[0]!.value} bpm</div>
  </div> : null;
};

const formatDate = (v: string) => {
  const date = new Date(v);
  return date.getHours().toString().padStart(2, '0') + ':' + date.getMinutes().toString().padStart(2, '0');
  //return date.toLocaleTimeString();
};