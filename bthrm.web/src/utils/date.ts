export const timeDiff = (d1: Date, d2: Date) => {
  const timeIntervals = [31536000, 2628000, 604800, 86400, 3600, 60, 1];
  const intervalNames = ['year', 'month', 'week', 'day', 'hour', 'minute', 'second'];
  let diff = Math.floor(Math.abs(d2.getTime() - d1.getTime()) / 1000);
  return timeIntervals.reduce((p, c, idx) => {
    const current = Math.floor(diff > c ? diff / c : 0);
    diff = current > 0 || p.length > 0 ? diff % c : diff;
    return current > 0 ? [...p, current + ' ' + intervalNames[idx] + 's'] : p
  }, [] as string[]).filter((x, idx) => idx < 2).join(', ');
}

export const minutes = (d1: Date, d2: Date) => {
  const minutes = Math.abs(d1.getTime() - d2.getTime()) / 1000 / 60;
  return minutes;
}