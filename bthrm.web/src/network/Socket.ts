'use client';

import RequestPacketType from "./RequestPacketType";
import ResponsePacketType from "./ResponsePacketType";

let _socket: WebSocket;
const socket = () => {
  if (!_socket) {
    _socket = new WebSocket(process.env.NEXT_PUBLIC_WEBSOCKET_URL ?? `ws://${location.host.indexOf(':') > 0 ? location.host.substring(0, location.host.indexOf(':')) : location.host}:4001`);
    _socket.onmessage = onMessage;
    _socket.onclose = onClose;
  }
  return _socket;
};

const onMessage = (msg: MessageEvent<string>) => {
  try {
    const data = JSON.parse(msg.data);
    const eventName = `ws-ev-${data.type}`;
    const event = new CustomEvent(eventName, { detail: data.data });
    document.dispatchEvent(event);
  } catch (e) {
    console.error('error sending event for data: ', msg.data);
  }
}

const onClose = (_: CloseEvent) => {
  window.location.href = window.location.protocol + '//' + window.location.host;
}

const send = <T>(type: RequestPacketType, data: T, log = false) => {
  const s = socket();
  const str = JSON.stringify({ data, type });
  s.send(str);
  if (log) {
    console.log('sent ', RequestPacketType[type], str);
  }
}

const listen = <T>(event: ResponsePacketType, handler: (data: T) => void, log = false) => {
  const eventListener = (msg: CustomEvent<any>) => {
    if (log) {
      console.log('received ', ResponsePacketType[event], msg.detail);
    }
    handler(msg.detail as T);
  }
  const eventName = `ws-ev-${event}`;
  //@ts-expect-error -- typescript doesn't like custom events
  document.addEventListener(eventName, eventListener);
  //@ts-expect-error -- typescript doesn't like custom events
  return () => document.removeEventListener(eventName, eventListener);
}

export default socket;
export { socket, listen, send };