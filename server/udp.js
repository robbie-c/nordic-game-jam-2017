import dgram from 'dgram';

import { kServerPort } from './constants';

export default function createUdpServer () {
  const udpServer = dgram.createSocket('udp4');

  return new Promise((resolve, reject) => {
    udpServer.on('error', (err) => {
      console.log(`server error:\n${err.stack}`);
      udpServer.close();

      reject(err);
    });

    udpServer.on('message', (msg, rinfo) => {
      console.log(`server got udp: ${msg} from ${rinfo.address}:${rinfo.port}`);
      udpServer.send(msg, 0, msg.length, rinfo.port, rinfo.address);
    });

    udpServer.on('listening', () => {
      const address = udpServer.address();
      console.log(`server listening ${address.address}:${address.port}`);

      resolve(udpServer);
    });

    udpServer.bind(kServerPort);
  });
}
