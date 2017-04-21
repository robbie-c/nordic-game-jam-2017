import { Server } from 'ws';

import { kServerPort } from './constants';

let playerIdCounter = 0;

export default function createWsServer (onNewConnection, onMessage, onError) {
  const server = new Server({
    perMessageDeflate: false,
    port: kServerPort
  });

  server.on('connection', function connection (client) {
    client.playerId = playerIdCounter++;
    onNewConnection(client);
    client.on('message', function incoming (message) {
      client.send("got message");
      onMessage(client, message);
      console.log('server got ws: %s', message);
    });
    client.on('error', function error (err) {
      onError(client, err);
    });
  });
}
