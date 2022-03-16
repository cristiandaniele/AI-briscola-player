using System;
using System.Collections.Generic;
using System.Linq;

namespace clientTCP
{
    class MainClass
    {

        static public Turn parseTurn(string incomingData, Match myMatch, handCard myHand, bool print)
        {
            Turn currentTurn = new Turn();
            List<string> data = incomingData.Split(',').ToList();
            if (data[0].Contains("true"))
            {
                currentTurn.setTurn(true);
            }
            else
            {
                currentTurn.setTurn(false);
                return null;
            }
            if (data[1].Contains("null"))
            {
                currentTurn.setCard(null);
            }
            else
            {
                //DEVE RICEVERE LA CARTA GIUSTA IN MODO DA POTERLA LEGGERE. SISTEMARE SUL SERVER. GI
                List<string> result = data[1].Split(',').ToList();
                var _seed = data[1];
                var seed = _seed.Split(':');
                var finalSeed = seed[2];
                finalSeed = finalSeed.Trim('"');
                var _val = data[2];
                var val = _val.Split(':');
                var final_val = val[1];
                final_val = final_val.Trim('"');
                var _points = data[4];
                var points = _points.Split(':');
                var final_points = points[1];
                final_points = final_points.Trim('"');

                var _pow = data[6];
                var pow = _pow.Split(':');
                var final_pow = pow[1];
                final_pow = final_pow.Trim('"');
                final_pow = final_pow.Trim('}');

                Card cardOnTable = new Card(final_val, finalSeed, 0, false, Convert.ToInt32(final_points), Convert.ToInt32(final_pow), myHand.getBriscolaSeed());
                myMatch.addCard(cardOnTable);
                currentTurn.setCard(cardOnTable);
            }
            if (print)
                currentTurn.printTurn();
            return currentTurn;
        }


        //devo passare solamente l'istanza di decision tree
        static public string attackTree(DecisionTree myTree, optimizeCard myOptimizator, Match myMatch, handCard myHand)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("SONO QUI SU ATTACK TREE");
            int outputTree = myTree.attack_tree();
            Card outputCard = myOptimizator.optimazeAttack(outputTree);

            string tmp = outputCard.calculateHash();
            myMatch.addCard(outputCard);
            myHand.removeCard(outputCard);
            return tmp;
        }

        //devo vedere cosa passare
        static public string defenseTree(DecisionTree myTree, optimizeCard myOptimizator, handCard myHand, Match myMatch)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("SONO QUI SU DEFENCE TREE");
            int outputTree = myTree.defence_tree();
            Card outputCard = myOptimizator.optimizeDef(outputTree);
            string tmp = outputCard.calculateHash();
            myMatch.addCard(outputCard);
            myHand.removeCard(outputCard);
            return tmp;
        }
        static public string playCard(Turn currentTurn, handCard myHand, Match myMatch, DecisionTree myTree, optimizeCard myOptimizator)
        {
            string hashToSend = "-1";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Stampo current turn");
            currentTurn.printTurn();

            //aggiorno alberi e optimizzatore con la situazione attuale
            myTree.updateState(myMatch, currentTurn, myHand);
            myOptimizator.updateState(myHand, currentTurn, myMatch);

            if (currentTurn.getCard() == null)
            {
                Console.WriteLine("gioco per primo");
                hashToSend = attackTree(myTree, myOptimizator, myMatch, myHand);
                //Sono il primo a dover giocare
            }
            else
            {
                Console.WriteLine("gioco per secondo");
                hashToSend = defenseTree(myTree, myOptimizator, myHand, myMatch);
                //Devo rispondere alla carta dell'avversario
            }
            return hashToSend;
        }
        static public string playLastCard(Turn currentTurn, handCard myHand, Match myMatch)
        {
            Card outputCard = myHand.getCard(0);

            string tmp = outputCard.calculateHash();
            myMatch.addCard(outputCard);
            myHand.removeCard(outputCard);
            return tmp;
        }

        public static void Main(string[] args)
        {
            bool debug = false;


            Match myMatch = new Match();
            WrapperTCP myWp = new WrapperTCP("127.0.0.1", 8080);

            handCard myHand = new handCard();

            Turn currentTurn = null;

            ObjectToSend tmp;

            DecisionTree myTree = new DecisionTree();

            optimizeCard myOptimizator = new optimizeCard();

            int decision = -1;

            int winThisHand = -1;
            //inizio la comunicazione con il server
            myWp.StartConnection();

            //Connecting to lobby
            tmp = new ObjectToSend("Start", "Peppe");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            myWp.SendData(tmp.getFormattedData());
            string whoami = myWp.ReceiveData();
            Console.WriteLine(whoami);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[BREAKPOINT]Press start to get card");
            if (debug)
            {
                Console.ReadLine();
            }
            else
            {
                System.Threading.Thread.Sleep(200);
            }

            //Asking first cards
            tmp = new ObjectToSend("FirstHand", "NULL");
            Console.ForegroundColor = ConsoleColor.Cyan;
            myWp.SendData(tmp.getFormattedData());
            string _myHand = myWp.ReceiveData();
            myHand.parseFirstHand(_myHand);
            myHand.printHand();

            int cardsPlayed = 0;
            while (cardsPlayed <= 40)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Carte giocate: " + cardsPlayed);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("[BREAKPOINT]Press start to get turn");
                if (debug)
                {
                    Console.ReadLine();
                }
                else
                {
                    System.Threading.Thread.Sleep(200);
                }

                //Asking turn
                if (currentTurn == null)
                    Console.WriteLine("Waiting the adversary...");
                bool print = true;
                while (currentTurn == null)
                {
                    tmp = new ObjectToSend("GetTurn", "NULL");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    myWp.SendData(tmp.getFormattedData());
                    currentTurn = parseTurn(myWp.ReceiveData(print), myMatch, myHand, print);
                    print = false;
                    System.Threading.Thread.Sleep(1000);

                }

                if (cardsPlayed == 38)
                {
                    //ultima carta da giocare
                    Console.WriteLine("Ultima carta da giocare");
                    string hashToSend = playLastCard(currentTurn, myHand, myMatch); //da vedere la rimozione se funziona!
                    Console.WriteLine("Devo mandare questo hash " + hashToSend);
                    tmp = new ObjectToSend("SendCard", hashToSend);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    myWp.SendData(tmp.getFormattedData());
                    Console.ForegroundColor = ConsoleColor.DarkYellow;

                    //QUI SERVE ANCORA FARE SPIN LOCK, NON SO SE HA GIà GIOCATO L'ALTRO
                    Console.WriteLine("[BREAKPOINT]Press start to get winner");
                    if (debug)
                    {
                        Console.ReadLine();
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(200);
                    }

                    tmp = new ObjectToSend("GetHandWinner", null);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    myWp.SendData(tmp.getFormattedData());
                    string winner = myWp.ReceiveData();

                    //AGGIORNO I PUNTI
                    if (winner.Contains("vinto"))
                    {
                        var _tmp = winner.Split('/');
                        var _points = _tmp[1];
                        myMatch.updatePoints(int.Parse(_points));
                        Console.WriteLine(_tmp[0] + " punti: " + _points);
                    }
                    else
                    {
                        Console.WriteLine(winner);
                    }
                    break;
                }
                else if (cardsPlayed >= 34) //DA VEDERE!
                {
                    //penultima carta da giocare
                    Console.WriteLine("Penultima carta da giocare");

                    string hashToSend = playCard(currentTurn, myHand, myMatch, myTree, myOptimizator);

                    Console.WriteLine("Devo mandare questo hash " + hashToSend);
                    tmp = new ObjectToSend("SendCard", hashToSend);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    myWp.SendData(tmp.getFormattedData());
                    Console.ForegroundColor = ConsoleColor.DarkYellow;

                    //QUI SERVE ANCORA FARE SPIN LOCK, NON SO SE HA GIà GIOCATO L'ALTRO
                    Console.WriteLine("[BREAKPOINT]Press start to get winner");
                    if (debug)
                    {
                        Console.ReadLine();
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(200);
                    }

                    tmp = new ObjectToSend("GetHandWinner", null);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    myWp.SendData(tmp.getFormattedData());
                    string winner = myWp.ReceiveData();

                    //AGGIORNO I PUNTI
                    if (winner.Contains("vinto"))
                    {
                        var _tmp = winner.Split('/');
                        var _points = _tmp[1];
                        myMatch.updatePoints(int.Parse(_points));
                        Console.WriteLine(_tmp[0] + " punti: " + _points);
                    }
                    else
                    {
                        Console.WriteLine(winner);
                    }
                }
                else
                {
                    string hashToSend = playCard(currentTurn, myHand, myMatch, myTree, myOptimizator);
                    Console.WriteLine("Devo mandare questo hash " + hashToSend);
                    tmp = new ObjectToSend("SendCard", hashToSend);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    myWp.SendData(tmp.getFormattedData());
                    Console.ForegroundColor = ConsoleColor.DarkYellow;

                    //QUI SERVE ANCORA FARE SPIN LOCK, NON SO SE HA GIà GIOCATO L'ALTRO 

                    Console.WriteLine("[BREAKPOINT]Press start to get winner");
                    if (debug)
                    {
                        Console.ReadLine();
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(200);
                    }

                    tmp = new ObjectToSend("GetHandWinner", null);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    myWp.SendData(tmp.getFormattedData());
                    string winner = myWp.ReceiveData();

                    //AGGIORNO I PUNTI
                    if (winner.Contains("vinto"))
                    {
                        var _tmp = winner.Split('/');
                        var _points = _tmp[1];
                        myMatch.updatePoints(int.Parse(_points));
                        Console.WriteLine(_tmp[0] + " punti: " + _points);
                    }
                    else
                    {
                        Console.WriteLine(winner);
                    }

                    //CHIEDO LA PROSSIMA CARTA
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("[BREAKPOINT]Press start to receive a new card");
                    if (debug)
                    {
                        Console.ReadLine();
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(200);
                    }

                    tmp = new ObjectToSend("GiveMeACard", null);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    myWp.SendData(tmp.getFormattedData());
                    string rsp = myWp.ReceiveData();
                    myHand.parseSingleCard(rsp, myHand);

                }
                //AGGIORNO I PUNTI
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("[BREAKPOINT]Press start to see new hand");
                if (debug)
                {
                    Console.ReadLine();
                }
                else
                {
                    System.Threading.Thread.Sleep(200);
                }

                myHand.printHand();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("[BREAKPOINT]Press start to proceed with the next hand ");
                if (debug)
                {
                    Console.ReadLine();
                }
                else
                {
                    System.Threading.Thread.Sleep(200);
                }

                myMatch.printExpiredCards();
                //TO SEE
                currentTurn = null;
                //TO SEE
                cardsPlayed += 2;
            }
            Console.WriteLine("PARTITA FINITA:");


            Console.WriteLine(myMatch.getPoints());

            Console.WriteLine("[BREAKPOINT]Press start to finish");
            Console.ReadLine();
        }
    }
}
