import { call, put, takeEvery, takeLatest, throttle, select } from 'redux-saga/effects';

import * as actions from './actions';
import {
  selectUdpServer,
  selectPlayers,
  selectGameId,
  selectHidingPlace,
  selectAnyPlayersHidden,
  selectAllPlayersHidden,
  selectAdminIo
} from './selectors';
import {
  udpSend,
  wsSend,
  wsEmit,
  delaySeconds,
  getRandomInt,
  getRandomStartingPosition
} from './calls';
import {
  kGameStateUpdateTickMs,
  kAdminUpdateTickMs,
  kSecondsBetweenFirstHideAndRoundEnd,
  kSecondsBetweenRoundEndAndNextRoundStart,
  kNumHidingPlaces
} from '../constants';

function * wsConnection ({ client }) {
  const gameId = yield select(selectGameId);
  const hidingPlace = yield select(selectHidingPlace);
  const playerPosition = yield call(getRandomStartingPosition);

  const player = {
    playerPosition,
    playerDirection: {
      x: 1,
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

  const helloMessage = JSON.stringify({
    type: 'ServerToClientHelloMessage',
    id: player.id,
    gameId,
    playerPosition: player.playerPosition,
    playerDirection: player.playerDirection,
    playerVelocity: player.playerVelocity,
    frozen: player.frozen,
    hidingPlace
  });

  console.log('sending hello message to client ' + player.id);
  yield call(
    wsSend,
    client,
    helloMessage
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
  const gameId = yield select(selectGameId);
  if (gameId === message.gameId) {
    yield put({
      type: actions.PLAYER_STATE_UPDATE,
      playerId,
      message
    });
  } else {
    console.error('wrong game id! got ' + message.gameId + ' expected ' + gameId);
  }
}

function * playerStateUpdate () {
  const udpServer = yield select(selectUdpServer);
  const players = yield select(selectPlayers);
  const gameId = yield select(selectGameId);
  const anyPlayersHidden = yield select(selectAnyPlayersHidden);
  const allPlayersHidden = yield select(selectAllPlayersHidden);

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
    if (player.id >= 0) {
      if (player.udpAddr && player.udpPort) {
        yield call(udpSend, udpServer, player.udpAddr, player.udpPort, message);
      } else if (player.ws) {
        // TODO consider rate limiting this
        yield call(wsSend, player.ws, message);
      } else {
        console.log('no way to send state update');
      }
    }
  }

  if (players.length > 0) {
    if (!anyPlayersHidden) {
      const newAnyPlayersHidden = players.some(player => player.frozen);
      if (newAnyPlayersHidden) {
        yield put({type: actions.FIRST_PLAYER_HIDDEN});
      }
    }
    if (!allPlayersHidden) {
      const newAllPlayersHidden = players.every(player => player.frozen);
      if (newAllPlayersHidden) {
        yield put({type: actions.LAST_PLAYER_HIDDEN});
      }
    }
  }
}

function * timeoutGameEnd (action) {
  if (action.type !== actions.FIRST_PLAYER_HIDDEN && action.type !== actions.LAST_PLAYER_HIDDEN) {
    return;
  }
  switch (action.type) {
    case actions.FIRST_PLAYER_HIDDEN:
      yield call(delaySeconds, kSecondsBetweenFirstHideAndRoundEnd + kSecondsBetweenRoundEndAndNextRoundStart);
      break;
    case actions.LAST_PLAYER_HIDDEN:
      yield call(delaySeconds, kSecondsBetweenRoundEndAndNextRoundStart);
      break;
  }

  if (action.type !== actions.START_GAME) {
    yield put({type: actions.REQUEST_START_GAME});
  }
}

function * startGame () {
  const players = yield select(selectPlayers);
  const oldGameId = yield select(selectGameId);
  const hidingPlace = yield call(getRandomInt, 0, kNumHidingPlaces);

  const gameId = oldGameId < Number.MAX_SAFE_INTEGER ? oldGameId + 1 : 0;

  yield put({
    type: actions.START_GAME,
    gameId,
    hidingPlace
  });

  for (const player of players) {
    const playerPosition = yield call(getRandomStartingPosition);

    yield put({
      type: actions.RESET_PLAYER,
      playerPosition,
      id: player.id
    });

    const message = JSON.stringify({
      type: 'ServerToClientStartMessage',
      gameId,
      hidingPlace,
      playerPosition
    });

    if (player.ws) {
      yield call(
        wsSend,
        player.ws,
        message
      );
    }
  }
}

function * updateAdminClients(client) {
  const players = yield select(selectPlayers);

  const message = JSON.stringify({
    numPlayers: players.length,
    numFrozenPlayers: players.filter(player => player.frozen).length
  });

  yield call(wsEmit, client, message);
}

function * adminConnected ({client}) {
  yield * updateAdminClients(client);
}

function * adminClientUpdate () {
  const adminIo = yield select(selectAdminIo);
  yield * updateAdminClients(adminIo);
}

function * adminStartGame () {
  yield put({type: actions.REQUEST_START_GAME});
}

export default function * saga () {
  yield takeEvery(actions.WS_CONNECTION, wsConnection);
  yield takeEvery(actions.WS_MESSAGE, wsMessage);
  yield takeEvery(actions.UDP_MESSAGE, udpMessage);
  yield takeEvery(actions.WS_DISCONNECT, wsDisconnect);
  yield takeEvery(actions.ADMIN_START_GAME, adminStartGame);
  yield takeEvery(actions.REQUEST_START_GAME, startGame);
  yield takeLatest([actions.START_GAME, actions.FIRST_PLAYER_HIDDEN], timeoutGameEnd);
  yield throttle(kGameStateUpdateTickMs, [actions.ADD_PLAYER, actions.REMOVE_PLAYER, actions.PLAYER_STATE_UPDATE], playerStateUpdate);
  yield throttle(kAdminUpdateTickMs, [actions.ADD_PLAYER, actions.REMOVE_PLAYER, actions.PLAYER_STATE_UPDATE], adminClientUpdate);
  yield takeEvery(actions.ADMIN_CONNECTION, adminConnected);
}
