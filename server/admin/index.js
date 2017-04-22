/* global $, io */

$(function () {
  console.log('starting');
  var socket = io(window.location.host);
  socket.on('connect', function () {
    console.log('connected');
  });

  $('#start-game').on('click', function () {
    socket.emit('startGame');
  });

  socket.on('state change', function (data) {
    var message = JSON.parse(data);
    console.log('state change', message);
    $('#num-players').text(message.numPlayers);
    $('#num-happy-sardines').text(message.numFrozenPlayers);
  });
});
