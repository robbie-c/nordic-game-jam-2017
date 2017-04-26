import {kServerAdminPort} from './constants';
const SocketIo = require('socket.io');
const express = require('express');
const http = require('http');

export default function createAdminServer (onWsCreated, onConnection, onStartGamePressed) {
  const app = express();
  app.use(express.static('server/admin'));

  const httpServer = http.createServer(app);
  const io = new SocketIo();
  io.listen(httpServer);

  httpServer.listen(kServerAdminPort, function () {
    console.log('admin portal server listening on ' + kServerAdminPort + ' ...');
  });

  io.on('connection', function (client) {
    onConnection(client);

    client.on('startGame', function () {
      onStartGamePressed();
    });
  });

  onWsCreated(io);
}
