export function udpSend (udpServer, ipAddr, ipPort, message) {
  console.log('sending udp to ' + ipAddr + ' ' + ipPort);
  udpServer.send(message, 0, message.length, ipPort, ipAddr);
}
