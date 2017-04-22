import dgram from 'dgram';

import { kServerPort } from './constants';

export default function createUdpServer (onCreate, onMessage, onError) {
  const udpServer = dgram.createSocket('udp4');

  udpServer.on('error', (err) => {
    console.log(`server error:\n${err.stack}`);
    udpServer.close();
    onError(err);
  });

  udpServer.on('message', (msg, rinfo) => {
    console.log(`server got udp: ${msg} from ${rinfo.address}:${rinfo.port}`);
    onMessage(rinfo, JSON.parse(msg));
  });

  udpServer.on('listening', () => {
    const address = udpServer.address();
    console.log(`udp server listening ${address.address}:${address.port}`);
    onCreate(udpServer);
  });

  udpServer.bind(kServerPort);
}
