import { combineReducers } from 'redux';

import {
  UDP_CREATE,
  ADD_PLAYER,

} from './actions';

function udpReducer (state = null, action = {}) {
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
    default:
      return state;
  }
}

export default combineReducers({
  players: playerReducer,
  udp: udpReducer
});
