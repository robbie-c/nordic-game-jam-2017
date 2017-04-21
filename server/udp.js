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
    udpServer.send(msg, 0, msg.length, rinfo.port, rinfo.address);
    onMessage(rinfo, JSON.parse(msg));
  });

  udpServer.on('listening', () => {
    const address = udpServer.address();
    console.log(`server listening ${address.address}:${address.port}`);
    onCreate(udpServer);
  });

  udpServer.bind(kServerPort);
}


