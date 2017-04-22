import {kServerAdminPort} from './constants';
import {
  selectPlayers
} from './gameState/selectors';
const SocketIo = require('socket.io');
const express = require('express');
const http = require('http');

export default function createAdminServer (subscribe, getState, onStartGamePressed) {
  const app = express();
  app.use(express.static('server/admin'));

  const httpServer = http.createServer(app);
  const io = new SocketIo();
  io.listen(httpServer);

  httpServer.listen(kServerAdminPort, function () {
    console.log('admin portal server listening on ' + kServerAdminPort + ' ...');
  });

  io.on('connection', function (client) {
    updateAdminClient(client);

    client.on('startGame', function () {
      onStartGamePressed();
    });
  });

  function updateAdminClient (client) {
    const state = getState();
    const players = selectPlayers(state);

    const message = JSON.stringify({
      numPlayers: players.length,
      numFrozenPlayers: players.filter(player => player.frozen).length
    });

    console.log('update admin clients', message);
    client.emit('state change', message);
  }

  subscribe(() => {
    updateAdminClient(io);
  });
}
