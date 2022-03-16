var net = require("net");
var colors = require("colors");
var server = net.createServer();
var player1 = 0;
var player2 = 0;
var port = 8080;
var hands;
var P1 = null;
var P2 = null;

server.on("connection", function(socket) {
  var remoteAddress = socket.remoteAddress + ":" + socket.remotePort;

  socket.on("data", function(incomingData) {
    //current player
    var currentIP = utility.parseIP(remoteAddress);
    //have to parse incomingData
    var data = utility.parseIncomingData(incomingData);
    console.log(data.header);

    if (data.header == "Start") {
      //Setting players
      if (player1 == 0) {
        hands = [];
        ref.createDeck();
        hands = ref.createFirstHand();
        console.log("--->");
        console.log(hands);
        console.log("<---");
        player1 = utility.parseIP(remoteAddress);
        console.log("Player1: " + player1);
        console.log(data.payload + " connected to the lobby as Player1!");
        socket.write(data.payload + " connected to the lobby as Player1!");
        currentTurn = ref.getInitialTurn();
        console.log("PRIMO GET TURN= " + currentTurn);
      } else {
        if (player2 == 0) {
          player2 = utility.parseIP(remoteAddress);
          console.log("Player2: " + player2);
          console.log("Sending to player 2 this data:");
          socket.write(data.payload + " connected to the lobby as Player2");
        } else {
          console.log("Lobby is full".red);
          socket.write("Lobby is full");
        }
      }
    }
    if (data.header == "FirstHand") {
      if (currentIP == player1) {
        console.log("Sending to player 1 this data:");
        console.log(hands[0]);
        socket.write(JSON.stringify(hands[0]));
      } else {
        if (currentIP == player2) {
          console.log("richiesta in arrivo dal giocatore 2");
          console.log(hands[1]);
          socket.write(JSON.stringify(hands[1]));
        } else {
          console.log("Errore sulla determinazione del giocatore".red);
        }
      }
    }
    if (data.header == "GetTurn") {
      console.log("IL TURNO CHE HO E':" + ref.getTurn());
      if (currentIP == player1) {
        if (ref.getTurn() == 0) {
          console.log("IL GIOCATORE 1 DEVE DIFENDERSI DA QUESTA CARTA:");
          var _card = ref.getAttackCardPlayer2();
          console.log(_card);
          socket.write(utility.sendTurn(1, _card));
        } else {
          socket.write(utility.sendTurn(0, null));
        }
      } else {
        if (currentIP == player2) {
          if (ref.getTurn() == 1) {
            console.log("IL GIOCATORE 2 DEVE DIFENDERSI DA QUESTA CARTA:");
            var _card = ref.getAttackCardPlayer1();
            console.log(_card);
            socket.write(utility.sendTurn(1, _card));
          } else {
            socket.write(utility.sendTurn(0, null));
          }
        } else {
          console.log("Foreign into the match!".red);
        }
      }
    }
    if (data.header == "SendCard") {
      console.log("QUI RICEVO LE CARTE DEI GIOCATORI");
      if (currentIP == player1) {
        console.log("GIOCATORE 1:");
        console.log(data.payload);
        ref.setHash(data.payload, 1);
        ref.setTurn(1);
      } else {
        if (currentIP == player2) {
          console.log("GIOCATORE 2:");
          console.log(data.payload);
          ref.setHash(data.payload, 2);
          ref.setTurn(0);
        } else {
          console.log("Foreign into the match!".red);
        }
      }
    }
    if (data.header == "GetHandWinner") {
      var w = ref.getHandWinner().player;

      var card = ref.getHandWinner().card;

      var winnerCard = card.val + " " + card.seed;
      var _loserCard = ref.getLoserCard();
      var loserCard = _loserCard.val + " " + _loserCard.seed;
      if (currentIP == player1) {
        console.log("P1 ha chiesto");
        P1 = socket;
      }
      if (currentIP == player2) {
        console.log("P2 ha chiesto");
        P2 = socket;
        //faccio aspettare p2
      }
      if (P1 != null && P2 != null) {
        //hanno chiesto entrambi rispondo e li sblocco
        console.log("Sblocco entrambi");
        console.log("--->");
        console.log(w);
        console.log("<---");

        var res_winner;
        var res_loser;
        console.log("\n\n\nAAAAAAAA---->");
        console.log(loserCard);
        console.log("---->");

        if (w == 0) {
          res_winner = "Hai vinto Player1 contro " + loserCard + "/";
          res_loser = "L'avversario ha preso con " + winnerCard + "\n";

          P1.write(res_winner + ref.getPoints());
          P2.write(res_loser);

          ref.setTurn(0);
        } else {
          res_winner = "Hai vinto Player2 contro " + loserCard + "/";
          res_loser = "L'avversario ha preso con " + winnerCard + "\n";
          P1.write(res_loser);
          P2.write(res_winner + ref.getPoints());
          ref.setTurn(1);
        }
        P1 = null;
        P2 = null;
      }
    }
    if (data.header == "GiveMeACard") {
      if (currentIP == player1) {
        socket.write(JSON.stringify(ref.getOneCard(0)));
      } else {
        if (currentIP == player2) {
          socket.write(JSON.stringify(ref.getOneCard(1)));
        } else {
          console.log("Foreign into the match!".red);
        }
      }
    }
    //send
  });
  socket.on("close", function() {
    console.log("connection closed");
  });
  socket.on("error", function(err) {
    console.log("error: " + err.message);
  });
});

var ref = require("./modules/referee");
var utility = require("./modules/utility");

server.listen(port, function(err) {
  if (err) {
    console.log("Errore: ", err);
  }
  console.log("Server avviato sulla porta %s".green, port);
});

server.on("error", err => console.log("An error occured".red + err));
