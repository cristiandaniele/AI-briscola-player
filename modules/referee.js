//GLOBAL VARIABLES
const sha256 = require("sha256");
var finish = false;
var turn = -1;
var HandWinner = {
  player: -1,
  card: -1
};
var loserCard = -1;
var val = [];
var seed = [];
var secureNumbers = [];
var deck = [];
var briscola;
var handPlayer1 = [];
var handPlayer2 = [];
var actualTurn; //0 or 1
var actualHand;
var pointThisHand = -1;
var firstPlayerHash = -1;
var secondPlayerHash = -1;
var attackCardPlayer1 = null;
var attackCardPlayer2 = null;
var cardToSend = [null, null];
var points = [0, 0]; //CONTENITORE DEI PUNTI DEI GIOCATORI

//----------------
var ref = {
  getAttackCardPlayer1: function() {
    var tmp = attackCardPlayer1;
    return tmp;
  },
  getLoserCard: function() {
    return loserCard;
  },
  getAttackCardPlayer2: function() {
    var tmp = attackCardPlayer2;
    return tmp;
  },
  clearAttackCardPlayer1: function() {
    attackCardPlayer1 = null;
  },
  clearAttackCardPlayer2: function() {
    attackCardPlayer2 = null;
  },
  compare: function(a, b) {
    if (a.random > b.random) return 1;
    return 0;
  },
  clearMatch: function() {
    val = [];
    finish = false;
    seed = [];
    secureNumbers = [];
    deck = [];
    handPlayer1 = [];
    handPlayer2 = [];
  },
  createDeck: function(req, res, next) {
    val = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
    punti = [11, 0, 10, 0, 0, 0, 0, 2, 3, 4];
    seed = ["Oro", "Coppe", "Mazze", "Spade"];

    for (let i = 0; i < 40; i++) {
      var rand = 0;
      do {
        rand = Math.floor(Math.random() * 10000);
      } while (secureNumbers.includes(rand));
      secureNumbers.push(rand);
    }
    let tmp = 0;
    seed.forEach((seme, i) => {
      val.forEach((valore, j) => {
        deck.push({
          seed: seme,
          val: valore,
          random: secureNumbers[tmp],
          points: punti[j],
          valid: true
        });
        tmp++;
      });
    });
    deck.sort(this.compare);
  },
  prova: function() {
    // this.createDeck();
    // this.createFirstHand();
    // this.getCard();
  },
  getBriscola: function() {
    return briscola;
  },
  calculateHash: function(card, hash) {
    var input = card.val + card.seed + card.random;

    console.log("confronto " + input + " con " + hash);
    if (input == hash) return true;
    return false;
  },
  setHash: function(hash, player) {
    //su turn ho chi ha cominciato a giocare
    console.log("\n\n\n\n---->attenziona questo hash");
    console.log(hash);
    if (player == 1) {
      firstPlayerHash = hash;
      var i = 0;
      handPlayer1.forEach((element, i) => {
        if (element) {
          var result = ref.calculateHash(element, firstPlayerHash);
          if (result) {
            console.log("---->La carta giocata dal player 1 è:");
            attackCardPlayer1 = element;
            console.log(attackCardPlayer1);
            handPlayer1 = this.shiftCard(handPlayer1, i);
            i++;
          }
        } else {
          console.log("QUI è L'ERRORE, HAND PLAYER1 CONTIENE:");
          console.log(handPlayer1);
        }
      });
    } else {
      if (player == 2) {
        secondPlayerHash = hash;
        var i = 0;
        handPlayer2.forEach((element, i) => {
          if (element) {
            var result = ref.calculateHash(element, secondPlayerHash);
            if (result) {
              console.log("---->La carta giocata dal player 2 è:");
              attackCardPlayer2 = element;
              console.log(attackCardPlayer2);
              console.log("HAND PLAYER 2 PRIMA");
              console.log(handPlayer2);
              console.log("HAND PLAYER 2 DOPO");
              handPlayer2 = this.shiftCard(handPlayer2, i);
              console.log(handPlayer2);
              i++;
            }
          } else {
            console.log("QUI è L'ERRORE, HAND PLAYER2 CONTIENE:");
            console.log(handPlayer2);
          }
        });
      }
    }
    if (firstPlayerHash != -1 && secondPlayerHash != -1) {
      console.log("entrambi hanno giocato, tocca decidere chi ha vinto");
      var cardsToEvaluate = [attackCardPlayer1, attackCardPlayer2];
      console.log("Carte da valutare:\n");
      console.log(cardsToEvaluate);
      firstPlayerHash = secondPlayerHash = -1;
      var allDataAboutWinner = this.setWinnerHand(cardsToEvaluate, turn);
      HandWinner.player = allDataAboutWinner._winner;

      pointThisHand = allDataAboutWinner._point;
      firstPlayerHash = secondPlayerHash = -1;
      attackCardPlayer1 = attackCardPlayer2 = null;
    }
  },
  shiftCard: function(hand, index) {
    var tmp = hand[0];
    hand[0] = hand[index];
    hand[index] = tmp;
    return hand;
  },
  otherTurn: function(turn) {
    var x = turn == 1 ? 0 : 1;
    return x;
  },
  setWinnerHand: function(cards, turn) {
    //Nessuna briscola sul tavolo
    console.log("Le carte da valutare sono:");
    console.log(cards);
    console.log("La briscola è:");
    console.log(briscola);

    //MI SERVE SAPERE CHI è STATO IL PRIMO A GIOCARE, NON DI CHI è IL TURNO ATTUALE
    console.log("Il turno è : " + turn);

    var _turn = this.otherTurn(turn);
    var winner = -1;
    if (cards[_turn].seed != cards[this.otherTurn(_turn)].seed) {
      if (
        cards[_turn].seed != briscola.seed &&
        cards[this.otherTurn(_turn)].seed != briscola.seed
      ) {
        console.log("--->A<---");
        winner = _turn;
      } else {
        if (
          cards[_turn].seed == briscola.seed &&
          cards[this.otherTurn(_turn)].seed != briscola.seed
        ) {
          console.log("--->B<---");
          winner = _turn;
        }
        if (
          cards[_turn].seed == briscola.seed &&
          cards[this.otherTurn(_turn)].seed != briscola.seed
        ) {
          console.log("--->C ---DA ATTENZIONARE--- <---");
          // winner = this.otherTurn(turn);
        }
        if (
          cards[_turn].seed != briscola.seed &&
          cards[this.otherTurn(_turn)].seed == briscola.seed
        ) {
          winner = this.otherTurn(_turn);
        }
      }
    }
    if (cards[_turn].seed == cards[this.otherTurn(_turn)].seed) {
      if (cards[_turn].pow == cards[this.otherTurn(_turn)].pow) {
        console.log("--->D<---");
        winner = _turn;
      } else {
        console.log("--->E<---");
        winner =
          cards[_turn].pow > cards[this.otherTurn(_turn)].pow
            ? _turn
            : this.otherTurn(_turn);
      }
    }
    if (winner == -1) {
      console.log("QUI NON DOVEVI ENTRARCI");
    }
    //assegno i punti al player che ha vinto
    var allPoints = cards[0].points + cards[1].points;
    points[winner] += allPoints;
    this.printPoints();

    turn = winner;
    console.log("[IMPORTANTE]qui aggiorno il turno: ");
    console.log(turn);
    console.log("[IMPORTANTE]");

    //QUI POTREI METTERE DA PARTE LE CARTE DA DARE DOPO AVENDO GIA' IL VINCITORE
    cardToSend[winner] = deck.pop();
    cardToSend[this.otherTurn(winner)] = deck.pop();
    HandWinner.card = cards[winner];
    loserCard = cards[this.otherTurn(winner)];
    console.log("---CARD TO SEND----");
    console.log(cardToSend);
    if (winner == 0) {
      handPlayer1[0] = cardToSend[winner];
    }
    if (winner == 1) {
      handPlayer2[0] = cardToSend[this.otherTurn(winner)];
    }
    console.log("---END CARD TO SEND---");
    var data = {
      _winner: winner,
      _point: allPoints
    };

    return data;
  },
  getOneCard: function(player) {
    if (player == 0) {
      console.log("Il giocatore 1 adesso ha carte:");
      console.log(handPlayer1);
      handPlayer1[0] = cardToSend[player];
    }
    if (player == 1) {
      console.log("Il giocatore 2 adesso ha carte:");
      console.log(handPlayer2);
      handPlayer2[0] = cardToSend[player];
    }
    return cardToSend[player];
  },
  printPoints: function() {
    console.log("Player 1 ha: " + points[0] + " punti");
    console.log("Player 2 ha: " + points[1] + " punti");
  },
  getPoints() {
    return pointThisHand;
  },
  getHandWinner: function() {
    return HandWinner;
  },
  addPow: function(vector, briscola) {
    vector.forEach(element => {
      switch (element.val) {
        case 1:
          element.pow = 12;
          break;
        case 2:
          element.pow = 2;
          break;
        case 3:
          element.pow = 11;
          break;
        case 4:
          element.pow = 4;
          break;
        case 5:
          element.pow = 5;
          break;
        case 6:
          element.pow = 6;
          break;
        case 7:
          element.pow = 7;
          break;
        case 8:
          element.pow = 8;
          break;
        case 9:
          element.pow = 9;
          break;
        case 10:
          element.pow = 10;
          break;
      }
      if (element.seed == briscola.seed) {
        element.pow *= 2;
      }
    });
  },
  createFirstHand: function() {
    console.log("deck prima");
    console.log(deck);
    for (let i = 0; i < 3; i++) {
      handPlayer1[i] = deck.pop();
      handPlayer2[i] = deck.pop();
    }
    briscola = deck.pop();

    // TO SEE
    // devo fare lo switch di tutto il mazzo e mettere la briscola in posizione  0 1 2 3 ->

    var _deck = [];
    _deck[0] = briscola;
    deck.forEach((element, i) => {
      _deck[i + 1] = deck[i];
    });

    console.log("deck dopo");
    deck = _deck;
    console.log(deck);
    // deck[deck.length] = briscola;

    this.addPow(handPlayer1, briscola);
    this.addPow(handPlayer2, briscola);
    this.addPow(deck, briscola);
    let tmp = JSON.parse(JSON.stringify(briscola)); //deep copy
    delete tmp.random;
    delete tmp.valid;
    handPlayer1.push(tmp);
    handPlayer2.push(tmp);
    var firstHands = [];
    firstHands.push(handPlayer1);
    firstHands.push(handPlayer2);
    return firstHands;
  },
  getCard: function() {
    var cardToSend = 0;
    if (deck.length >= 1) {
      cardToSend = deck.pop();
    } else {
      if (!finish) {
        console.log("Ultima presa");
        cardToSend = briscola;
        finish = true;
      } else {
        console.log("Partita finita");
      }
    }
    console.log("card to send:");
    console.log(cardToSend);
  },
  getInitialTurn: function() {
    turn = Math.floor(Math.random() * 2);
    return turn;
  },
  getTurn: function() {
    return turn;
  },
  setTurn: function(x) {
    turn = x;
  }
};
//END DB MODULE
module.exports = ref;
