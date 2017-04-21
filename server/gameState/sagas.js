import { call, put, takeEvery } from 'redux-saga/effects';

import * as actions from './actions';

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

  console.log('about to put');

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
      playerVelocity: player.playerVelocity
    })
  );

  console.log('should have emitted');
}

function * wsMessage ({message}) {
  handleMessage(message.playerId, message);
}

function * udpMessage ({message}) {
  handleMessage(message.playerId, message);
}

function * handleMessage (playerId, message) {
  switch (message.type) {
    case 'CLIENT_GAME_STATE_MESSAGE':
      handleClientGameStateMessage(playerId, message);
  }
}

function * handleClientGameStateMessage (playerId, message) {
}

export default function * saga () {
  console.log('sagas', actions.WS_CONNECTION);
  yield takeEvery(actions.WS_CONNECTION, wsConnection);
  yield takeEvery(actions.WS_MESSAGE, wsMessage);
  yield takeEvery(actions.UDP_MESSAGE, udpMessage);
}
