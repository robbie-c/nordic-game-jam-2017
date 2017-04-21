import { Server } from 'ws';

import { kServerPort } from './constants';

export default function createWsServer () {
  const server = new Server({
    perMessageDeflate: false,
    port: kServerPort
  });

  server.on('connection', function connection (client) {
    client.on('message', function incoming (message) {
      console.log('server got ws: %s', message);
      client.send(message);
    });
  });
}
