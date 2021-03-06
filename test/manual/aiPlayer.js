import dgram from 'dgram';
import {
  kServerAddr,
  kServerPort,
  kClientTestPort,
  kGameStateUpdateTickMs
} from '../../server/constants';
const WebSocket = require('ws');

const ws = new WebSocket('ws://' + kServerAddr + ':' + kServerPort, {
  perMessageDeflate: false
});

ws.on('open', () => {
  ws.send('something');
});

let playerId = -1;
let position;
let direction;
let velocity;
let gameId;

ws.on('message', (data) => {
  const message = JSON.parse(data);
  console.log('Recv', message);
  switch (message.type) {
    case 'ServerToClientHelloMessage':
      playerId = message.id;
      position = message.playerPosition;
      direction = message.playerDirection;
      velocity = message.playerVelocity;
      gameId = message.gameId;
      console.log('starting gameId is ', gameId);
      startSendingUdp();
      break;
    case 'ServerToClientStartMessage':
      gameId = message.gameId;
      console.log('new gameId is ', gameId);
      break;
    default:
      console.log('Unexpected message type: ' + message.type);
  }
});

function delay (ms) {
  return new Promise((resolve) => {
    setTimeout(resolve, ms);
  });
}

async function startSendingUdp () {
  const udpSocket = dgram.createSocket('udp4');
  udpSocket.bind(kClientTestPort);

  while (true) {
    // TODO change velocity as well as position?
    position = {
      x: position.x + 0.1,
      y: position.y,
      z: position.z
    };

    direction = {
      x: 0,
      y: 0,
      z: 1
    };

    const message = JSON.stringify({
      type: 'ClientGameStateMessage',
      id: playerId,
      gameId,
      playerPosition: position,
      playerDirection: direction,
      playerVelocity: velocity,
      frozen: false
    });

    console.log('Send', message);
    udpSocket.send(message, 0, message.length, kServerPort, kServerAddr);

    await delay(kGameStateUpdateTickMs);
  }
}
