import createTcpServer from './tcp';
import createUdpServer from './udp';
import createAdminServer from './admin';
import store from './gameState';
import * as actions from './gameState/actions';

const dispatch = store.dispatch;

function onWsConnection (client) {
  dispatch({
    type: actions.WS_CONNECTION,
    client
  });
}

function onWsMessage (client, message) {
  // TODO check player id matches

  dispatch({
    type: actions.WS_MESSAGE,
    message,
    client
  });
}

function onWsError (client, error) {
  console.error(error);
}

function onWsDisconnect (client) {
  dispatch({
    type: actions.WS_DISCONNECT,
    client
  });
}

function onUdpCreate (udpServer) {
  dispatch({
    type: actions.UDP_CREATE,
    udpServer
  });
}

function onUdpMessage (rinfo, message) {
  dispatch({
    type: actions.UDP_MESSAGE,
    rinfo,
    message
  });
}

function onUdpError (error) {
  console.error(error);
}

function onAdminStartGamePressed () {
  console.log('admin start game');
  dispatch({
    type: actions.ADMIN_START_GAME
  });
}

createTcpServer(onWsConnection, onWsMessage, onWsError, onWsDisconnect);
createUdpServer(onUdpCreate, onUdpMessage, onUdpError);
createAdminServer(store.subscribe, store.getState, onAdminStartGamePressed);
console.log('Created server');
