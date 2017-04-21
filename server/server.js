import createTcpServer from './tcp';
import createUdpServer from './udp';

createTcpServer();
createUdpServer();
console.log('Created server');