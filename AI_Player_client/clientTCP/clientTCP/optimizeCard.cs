using System;
using System.Collections.Generic;

namespace clientTCP
{
    public class optimizeCard
    {

        handCard myCards;
        Turn actualTurn;
        Card adversaryCard;
        Match actualMatch;

        public void updateState(handCard myCards,Turn actualTurn,Match actualMatch)
        {
            this.myCards = myCards;
            this.actualTurn = actualTurn;
            adversaryCard = actualTurn.getCard();
            this.actualMatch = actualMatch;
        }

        public Card optimazeAttack(int move)
        {
            Console.WriteLine("Sono su attacco, devo");
            if (move == 0) Console.WriteLine(" tirare liscio o briscola");
            else
            {
                Console.WriteLine(" tirare carico o punti");
            }
            //su move ho l'output dell'albero 0-liscio o briscola / 1-carico o punti
            List<Card> possibleOutput = new List<Card>();
            List<Card> myHand = myCards.getAllCards();
            Card possibleCard = null;
            if (move == 0)
            {
                //liscio o briscola nell'albero di attacco
                int max = -1;
                foreach(Card x in myHand)
                {
                    if (x.getPunteggio() == 0 && x.checkBriscola() == false)
                    {
                        //è un liscio
                        possibleOutput.Add(x);
                        if (actualMatch.checkCaricoSeed(x) >= max)
                        {
                            possibleCard = x;
                            max = actualMatch.checkCaricoSeed(x);
                        }
                    }
                }
                if (possibleCard == null)
                {
                    int min = 20;
                    foreach (Card x in myHand)
                    {
                        //CHECK DI BRISCOLE BASSE
                        if (x.checkBriscola() && (x.getVal() == "2" || x.getVal() == "4" || x.getVal() == "5" || x.getVal() == "6"))
                        {
                            //è una briscola bassa
                            if( Convert.ToInt16(x.getVal()) <= min)
                            {
                                possibleCard = x;
                                min = Convert.ToInt16(x.getVal());
                            }
                        }

                    }
                }

                if (possibleCard == null)
                {
                    int min = 100;
                    //butto la carta che vale meno pow
                    foreach(Card x in myHand)
                    {
                        if (x.getPow() <= min)
                        {
                            possibleCard = x;
                            min = x.getPow();
                        }
                    }
                }
                
            }
            else
            {
                int min = 400;
                //carico o punti nell'albero di attacco
                foreach(Card x in myHand)
                {
                    if (x.getPow() <= min)
                    {
                        possibleCard = x;
                        min = x.getPow();
                    }
                } 
            }
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Tiro questa carta:");
            if (possibleCard == null)
            {
                Console.WriteLine("QUI NON DOVRESTI ENTRARCI, ATTENZIONA QUESTE CONDIZIONI");
                possibleCard = myHand[0];
            }
            if (possibleCard == null)
            {
                possibleCard = myHand[0];
            }
            possibleCard.printCard();
            return possibleCard;
        }

        public Card optimizeDef(int move)
        {
            List<Card> myHand = myCards.getAllCards();
            Card possibleCard = null;
            Card enemyCard = actualTurn.getCard();
            //move == 0 se lascio move == 1 prendo
            Console.WriteLine("Sono su difesa, devo");
            if (move == 0) Console.WriteLine(" lasciare");
            else
            {
                Console.WriteLine(" prendere");
            }
            if (move == 0)
            {
                int min = 400;
                //carico o punti nell'albero di attacco
                foreach (Card x in myHand)
                {
                    if (x.getPow() <= min)
                    {
                        possibleCard = x;
                        min = x.getPow();
                    }
                }
                
            }
            else
            {
                int max = -1;
                int min = 30;
                //se ha briscola butto il meno possibile
                if (enemyCard.checkBriscola())
                {
                    if (enemyCard.getPunteggio() == 10)
                    {
                        //se il nemico tira il 3
                        //controllo se ho l'asso
                        foreach (Card x in myHand)
                        {
                            if (x.getPunteggio() == 11)
                            {
                                possibleCard = x;
                            }
                        }
                    }
                    else {
                        foreach (Card x in myHand)
                        {
                            if (x.getPow() < min)
                            {
                                possibleCard = x;
                                min = x.getPow();
                            }
                        }
                    }
                }
                else
                {
                    //preferisco avanzare sempre in carta
                    foreach (Card x in myHand)
                    {

                        if (enemyCard.getSeed() == x.getSeed())
                        {
                            if (x.getPow() > enemyCard.getPow())
                            {
                                if (x.getPow()>max)
                                {
                                    possibleCard = x;
                                    max = x.getPow();
                                }
                                
                            }
                        }
                    }
                }
                if (possibleCard == null)
                {
                    int _min = 40;
                    //non ho potuto avanzare in carta quindi butto la briscola più piccola
                    foreach (Card x in myHand)
                    {

                        if (x.checkBriscola() && x.getPow() <_min)
                        {
                            possibleCard = x;
                        }
                    }
                }

                
            }

            Console.WriteLine("Tiro questa carta:");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (possibleCard == null)
            {
                possibleCard = myHand[0];
            }
            possibleCard.printCard();
            return possibleCard;
        }

    }
}
