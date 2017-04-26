import { Server } from 'ws';
import { kServerPort } from './constants';
const express = require('express');
const http = require('http');

let playerIdCounter = 0;

export default function createWsServer (onNewConnection, onMessage, onError, onDisconnect) {
  const app = express();
  app.use(express.static('server/html'));

  const httpServer = http.createServer(app);

  const server = new Server({
    server: httpServer
  });

  server.on('connection', (client) => {
    client.playerId = playerIdCounter++;
    onNewConnection(client);
    client.on('message', (message) => {
      onMessage(client, JSON.parse(message));
    });
    client.on('error', (err) => {
      onError(client, err);
    });
    client.on('close', () => {
      console.log('close');
      onDisconnect(client);
    });
  });

  const port = process.env.PORT || kServerPort;
  httpServer.listen(port, function () {
    console.log(`ws server listening on port ${port} ...`);
  });
}
