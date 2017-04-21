import { call, put, takeEvery, throttle, select } from 'redux-saga/effects';

import * as actions from './actions';
import {
  selectUdpServer,
  selectPlayers
} from './selectors';
import {
  udpSend
} from './calls';

import { kClientPort } from '../constants';

function * wsConnection ({ client }) {
  const player = {
    playerPosition: {
      x: 0,
      y: 0,
      z: 0
    },
    playerDirection: {
      x: 0,
      y: 0,
      z: 0
    },
    playerVelocity: {
      x: 0,
      y: 0,
      z: 0
    },
    frozen: false,
    ws: client,
    id: client.playerId
  };

  yield put({
    type: actions.ADD_PLAYER,
    player
  });

  function send (message) {
    client.send(message);
  }

  console.log('sending hello message to client ' + player.id);
  yield call(
    send,
    JSON.stringify({
      type: 'ServerToClientHelloMessage',
      id: player.id,
      playerPosition: player.playerPosition,
      playerDirection: player.playerDirection,
      playerVelocity: player.playerVelocity,
      frozen: player.frozen
    })
  );
}

function * wsMessage ({message}) {
  yield * handleMessage(message.id, message);
}

function * udpMessage ({message, rinfo}) {
  yield * handleMessage(message.id, message);
}

function * handleMessage (playerId, message) {
  switch (message.type) {
    case 'ClientGameStateMessage':
      yield * handleClientGameStateMessage(playerId, message);
  }
}

function * handleClientGameStateMessage (playerId, message) {
  console.log('handle client game state message ' + playerId);
  yield put({
    type: actions.PLAYER_STATE_UPDATE,
    playerId,
    message
  });
}

function * playerStateUpdate () {
  const udpServer = yield select(selectUdpServer);
  const players = yield select(selectPlayers);

  const message = JSON.stringify({
    type: 'ServerGameStateMessage',
    clients: players.map((player) => {
      return {
        id: player.id,
        playerPosition: player.playerPosition,
        playerDirection: player.playerDirection,
        playerVelocity: player.playerVelocity,
        frozen: player.frozen
      };
    })
  });

  for (const player of players) {
    console.log(player.id, player.udpAddr, player.udpPort);
    if (player.id >= 0 && player.udpAddr && player.udpPort) {
      yield call(udpSend, udpServer, player.udpAddr, player.udpPort, message);
    }
  }
}

export default function * saga () {
  yield takeEvery(actions.WS_CONNECTION, wsConnection);
  yield takeEvery(actions.WS_MESSAGE, wsMessage);
  yield takeEvery(actions.UDP_MESSAGE, udpMessage);
  yield throttle(1000, [actions.ADD_PLAYER, actions.PLAYER_STATE_UPDATE], playerStateUpdate);
}
