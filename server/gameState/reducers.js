import { combineReducers } from 'redux';

import {
  UDP_CREATE,
  ADD_PLAYER,
  PLAYER_STATE_UPDATE,
  UDP_MESSAGE
} from './actions';

function udpServerReducer (state = null, action = {}) {
  switch (action.type) {
    case UDP_CREATE:
      return action.udpServer;
    default:
      return state;
  }
}

function playerReducer (state = [], action = {}) {
  switch (action.type) {
    case ADD_PLAYER:
      return [
        ...state,
        action.player
      ];
    case PLAYER_STATE_UPDATE: {
      const {message, playerId} = action;
      return state.map((player) => {
        if (player.id === playerId) {
          return {
            ...player,
            playerPosition: message.playerPosition,
            playerDirection: message.playerDirection,
            playerVelocity: message.playerVelocity,
            frozen: message.frozen
          };
        } else {
          return player;
        }
      });
    }
    case UDP_MESSAGE: {
      const {message, rinfo} = action;
      return state.map((player) => {
        if (player.id === message.id) {
          return {
            ...player,
            udpAddr: rinfo.address,
            udpPort: rinfo.port
          };
        } else {
          return player;
        }
      });
    }
    default:
      return state;
  }
}

export default combineReducers({
  players: playerReducer,
  udpServer: udpServerReducer
});
