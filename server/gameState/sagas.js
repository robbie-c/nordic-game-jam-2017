import { call, put, takeEvery, takeLatest, throttle, select } from 'redux-saga/effects';

import * as actions from './actions';
import {
  selectUdpServer,
  selectPlayers,
  selectGameId,
  selectAnyPlayersHidden
} from './selectors';
import {
  udpSend,
  wsSend,
  delaySeconds
} from './calls';
import {
  kGameStateUpdateTickMs,
  kSecondsBetweenFirstHideAndRoundEnd,
  kSecondsBetweenRoundEndAndNextRoundStart
} from '../constants';

function * wsConnection ({ client }) {
  const gameId = yield select(selectGameId);

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

  console.log('sending hello message to client ' + player.id);
  yield call(
    wsSend,
    client,
    JSON.stringify({
      type: 'ServerToClientHelloMessage',
      id: player.id,
      gameId,
      playerPosition: player.playerPosition,
      playerDirection: player.playerDirection,
      playerVelocity: player.playerVelocity,
      frozen: player.frozen
    })
  );
}

function * wsDisconnect ({client}) {
  yield put({
    type: actions.REMOVE_PLAYER,
    id: client.playerId
  });
}

function * wsMessage ({message}) {
  yield * handleMessage(message.id, message);
}

function * udpMessage ({message}) {
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
  const gameId = yield select(selectGameId);
  const anyPlayersHidden = yield select(selectAnyPlayersHidden);

  const message = JSON.stringify({
    type: 'ServerGameStateMessage',
    gameId,
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
    if (player.id >= 0 && player.udpAddr && player.udpPort) {
      yield call(udpSend, udpServer, player.udpAddr, player.udpPort, message);
    }
  }

  if (!anyPlayersHidden) {
    const newAnyPlayersHidden = players.some(player => player.frozen);
    if (newAnyPlayersHidden) {
      yield put({type: actions.FIRST_PLAYER_HIDDEN});
    }
  } else {
    const allPlayersHidden = players.every(player => player.frozen);
    if (allPlayersHidden) {
      yield put({type: actions.LAST_PLAYER_HIDDEN});
    }
  }
}

function * timeoutGameEnd (action) {
  switch (action.type) {
    case actions.FIRST_PLAYER_HIDDEN:
      call(delaySeconds, kSecondsBetweenFirstHideAndRoundEnd + kSecondsBetweenRoundEndAndNextRoundStart);
      break;
    case actions.LAST_PLAYER_HIDDEN:
      call(delaySeconds, kSecondsBetweenRoundEndAndNextRoundStart);
      break;
  }

  if (action.type !== actions.START_GAME) {
    yield put({type: actions.START_GAME});
  }
}

function * startGame () {
  const players = yield select(selectPlayers);
  const gameId = yield select(selectGameId);

  const message = JSON.stringify({
    type: 'ServerToClientStartMessage',
    gameId
  });

  for (const player of players) {
    if (player.ws) {
      yield call(
        wsSend,
        player.ws,
        message
      );
    }
  }
}

function * adminStartGame () {
  yield put({type: actions.START_GAME});
}

export default function * saga () {
  yield takeEvery(actions.WS_CONNECTION, wsConnection);
  yield takeEvery(actions.WS_MESSAGE, wsMessage);
  yield takeEvery(actions.UDP_MESSAGE, udpMessage);
  yield takeEvery(actions.WS_DISCONNECT, wsDisconnect);
  yield takeEvery(actions.ADMIN_START_GAME, adminStartGame);
  yield takeEvery(actions.START_GAME, startGame);
  yield takeLatest([actions.START_GAME, actions.FIRST_PLAYER_HIDDEN, actions.LAST_PLAYER_HIDDEN], timeoutGameEnd);
  yield throttle(kGameStateUpdateTickMs, [actions.ADD_PLAYER, actions.PLAYER_STATE_UPDATE], playerStateUpdate);
}
