import { combineReducers } from 'redux';

import {
  UDP_CREATE,
  ADD_PLAYER,
  REMOVE_PLAYER,
  PLAYER_STATE_UPDATE,
  UDP_MESSAGE,
  ADMIN_START_GAME,
  FIRST_PLAYER_HIDDEN,
  LAST_PLAYER_HIDDEN
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
    case REMOVE_PLAYER: {
      const { id } = action;
      return state.filter((player) => player.id !== id);
    }
    default:
      return state;
  }
}

function gameIdReducer (state = 0, action = {}) {
  switch (action.type) {
    case ADMIN_START_GAME: {
      if (state < Number.MAX_SAFE_INTEGER) {
        return state + 1;
      } else {
        return Number.MIN_SAFE_INTEGER;
      }
    }
    default:
      return state;
  }
}

function anyPlayersHiddenReducer (state = false, action = {}) {
  switch (action.type) {
    case ADMIN_START_GAME: {
      return false;
    }
    case FIRST_PLAYER_HIDDEN: {
      return true;
    }
    default:
      return state;
  }
}

function allPlayersHiddenReducer (state = false, action = {}) {
  switch (action.type) {
    case ADMIN_START_GAME: {
      return false;
    }
    case LAST_PLAYER_HIDDEN: {
      return true;
    }
    default:
      return state;
  }
}

export default combineReducers({
  players: playerReducer,
  udpServer: udpServerReducer,
  gameId: gameIdReducer,
  anyPlayersHidden: anyPlayersHiddenReducer,
  allPlayersHidden: allPlayersHiddenReducer
});
