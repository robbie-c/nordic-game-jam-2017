import {
  kMaxStartingX,
  kMinStartingX,
  kMaxStartingZ,
  kMinStartingZ
} from '../constants';

export function udpSend (udpServer, ipAddr, ipPort, message) {
  console.log('sending udp to ' + ipAddr + ' ' + ipPort);
  udpServer.send(message, 0, message.length, ipPort, ipAddr);
}

export function wsSend (client, message) {
  client.send(message);
}

export function delayMs (ms) {
  return new Promise((resolve) => {
    setTimeout(resolve, ms);
  });
}

export function delaySeconds (seconds) {
  return delayMs(seconds * 1000);
}

// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Math/random
export function getRandomInt (min, max) {
  min = Math.ceil(min);
  max = Math.floor(max);
  return Math.floor(Math.random() * (max - min)) + min;
}

function getRandomArbitrary(min, max) {
  return Math.floor(Math.random() * (max - min)) + min;
}

export function getRandomStartingPosition () {
  return {
    x: getRandomArbitrary(kMinStartingX, kMaxStartingX),
    y: 0,
    z: getRandomArbitrary(kMinStartingZ, kMaxStartingZ)
  };
}