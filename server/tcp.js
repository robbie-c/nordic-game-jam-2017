import { Server } from 'ws';

import { kServerPort } from './constants';

let playerIdCounter = 0;

export default function createWsServer (onNewConnection, onMessage, onError, onDisconnect) {
  const server = new Server({
    perMessageDeflate: false,
    port: kServerPort
  });

  server.on('connection', (client) => {
    client.playerId = playerIdCounter++;
    onNewConnection(client);
    client.on('message', (message) => {
      onMessage(client, message);
    });
    client.on('error', (err) => {
      onError(client, err);
    });
    client.on('close', () => {
      console.log('close');
      onDisconnect(client);
    });
  });
  server.on('listening', function () {
    console.log(`ws server listening on port ${kServerPort} ...`);
  });
}
