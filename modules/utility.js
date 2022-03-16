//SERVE CAPIRE CHI FA LE RICHIESTE, SE IL GIOCATORE 1 O IL GIOCATORE 2 (IP?)
//GLOBAL VARIABLES

//----------------
var utility = {
  parseIncomingData: function(incomingData) {
    var dataParsed = {};
    data = incomingData.toString().split(",");
    var header = data[0];
    var payload = data[1];
    header = header.toString().split(":");
    payload = payload.toString().split(":");
    dataParsed.header = header[1];
    dataParsed.payload = payload[1].slice(0, payload[1].length - 1);
    return dataParsed;
  },
  parseIP: function(ipPlayer) {
    var tmp = ipPlayer.toString().split(":");
    return tmp[4];
  },
  sendTurn: function(your, card) {
    var sendingData = {
      yourTurn: null,
      cardOnTheTable: null
    };
    if (your) {
      sendingData.yourTurn = true;
    }
    if (card) {
      sendingData.cardOnTheTable = card;
    }
    return JSON.stringify(sendingData);
  }
};
//END DB MODULE
module.exports = utility;
