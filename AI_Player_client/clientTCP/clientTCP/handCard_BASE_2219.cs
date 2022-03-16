using System;
using System.Collections.Generic;
using System.Linq;

namespace clientTCP
{
    public class handCard
    {
        private Card[] myHand = new Card[3];
        private Card briscola;

        //se ho liscio, se ho punti, se ho 3, se ho asso

        public handCard()
        {
        }

        public string getBriscolaSeed()
        {
            return briscola.getSeed();
        }
        public handCard(Card[] hand, Card b)
        {
            myHand = hand;
            briscola = b;
        }

        public List<Card> getAllCards()
        {
            return myHand.ToList();
        }
        public void removeCard(Card toRemove)
        {
            int index = -1;
            for (int i = 0; i < 3; i++)
            {
                if (myHand[i] == null)
                {
                    break;
                }
                if (myHand[i] == toRemove)
                {
                    //TO CHECK
                    Console.WriteLine("DA VEDERE");
                    index = i;
                }
            }

            Card tmp;
            tmp = myHand[0];
            myHand[index].valid = false;
            myHand[0] = myHand[index];
            myHand[index] = tmp;
        }
        public void parseFirstHand(string x)
        {
            //ci saranno 4 carte,le estrapolo
            List<string> result = x.Split(',').ToList();
            int j = 1;
            int i = 0;

            //adding last card
            var tmp_seed = result[18].Split(':');
            string tmpseed = tmp_seed[1];
            var tmp_val = result[19].Split(':');
            string tmpval = tmp_val[1];
            var tmp_points = result[20].Split(':');
            string tmppoints = tmp_points[1];
            tmpseed = tmpseed.Trim('"');
            tmpval = tmpval.Trim('"');
            tmppoints = tmppoints.Trim(']');
            tmppoints = tmppoints.Trim('}');
            briscola = new Card((tmpval), tmpseed, -1, false, Convert.ToInt32(tmppoints), -1, tmpseed);

            for (int card = 0; card < 18; card++)
            {
                var _seed = result[card].Split(':');
                string seed = _seed[1];
                var _val = result[card + 1].Split(':');
                string val = _val[1];
                var _random = result[card + 2].Split(':');
                string random = _random[1];
                var _points = result[card + 3].Split(':');
                string points = _points[1];
                var _valid = result[card + 4].Split(':');
                string valid = _valid[1];
                var _pow = result[card + 5].Split(':');
                string pow = _pow[1];
                card += 5;
                seed = seed.Trim('"');
                val = val.Trim('"');
                pow = pow.Trim('"');
                pow = pow.Trim('}');

                if ((card + 1) % 6 == 0)
                {
                    bool _tmpBool = false;
                    if (valid.Contains("true"))
                    {
                        _tmpBool = true;
                    }
                    Console.WriteLine(val + " " + seed + " " + random + " " + _tmpBool + " " + points + " " + pow + " ");
                    Card tmpCard = new Card(val, seed, Convert.ToInt32(random), _tmpBool, Convert.ToInt32(points), Convert.ToInt32(pow), briscola.getSeed());
                    myHand[i] = tmpCard;
                    i++;
                }
            }

        }
        public void parseSingleCard(string cards, handCard myHand1)
        {
            List<string> result = cards.Split(',').ToList();
            var _seed = result[0].Split(':');
            string seed = _seed[1]; //TODO genera un errore dimensione matrice quando ricevo la carta
            var _val = result[1].Split(':');
            string val = _val[1];
            var _random = result[2].Split(':');
            string random = _random[1];
            var _points = result[3].Split(':');
            string points = _points[1];
            var _valid = result[4].Split(':');
            string valid = _valid[1];
            var _pow = result[5].Split(':');
            string pow = _pow[1];
            seed = seed.Trim('"');
            val = val.Trim('"');
            pow = pow.Trim('"');
            pow = pow.Trim('}');
            myHand[0] = new Card((val), seed, Convert.ToInt32(random), true, Convert.ToInt32(points), Convert.ToInt32(pow), myHand1.getBriscolaSeed());
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Ho pescato la seguente carta:");
            myHand[0].printCard();

        }
        public void scarta(int index)
        {
            //to test
            myHand = myHand.Where((e, i) => i != index).ToArray();
        }
        public void printHand()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("[GAME]In mano ho le seguenti carte:");
            foreach (Card x in myHand)
            {
                x.printCard();
            }
            briscola.printCard();
        }
        public Card getCard(int index)
        {
            return myHand[index];
        }

        public bool checkLiscio()
        {
            for (int i = 0; i < 3; i++)
            {
                Card card = myHand[i];
                if (!card.checkBriscola() && card.getPunteggio() == 0) return true;
            }
            return false;
        }

        public bool checkPunti()
        {
            for (int i = 0; i < 3; i++)
            {
                Card card = myHand[i];
                if (!card.checkBriscola() && card.getPunteggio() > 0 && card.getPunteggio() < 10) return true;
            }
            return false;
        }

        public bool checkTre()
        {
            for (int i = 0; i < 3; i++)
            {
                Card card = myHand[i];
                if (card.checkBriscola() && card.getPunteggio() == 10) return true;
            }
            return false;
        }

        public bool checkAsso()
        {
            for (int i = 0; i < 3; i++)
            {
                Card card = myHand[i];
                if (card.checkBriscola() && card.getPunteggio() == 11) return true;
            }
            return false;
        }

        public bool checkBriscolaBassa()
        {
            for (int i = 0; i < 3; i++)
            {
                Card card = myHand[i];
                if (card.checkBriscola() && (card.getVal() == "2" || card.getVal() == "4" || card.getVal() == "5" || card.getVal() == "6")) return true;
            }
            return false;
        }

        public bool checkBriscolaMedia()
        {
            for (int i = 0; i < 3; i++)
            {
                Card card = myHand[i];
                if (card.checkBriscola() && (card.getVal() == "7" || card.getVal() == "8" || card.getVal() == "9" || card.getVal() == "10")) return true;
            }
            return false;
        }

        public bool checkAvanzo(Card target)
        {
            if (target.checkBriscola()) return false; //solo avanzo su non briscole?

            for (int i = 0; i < 3; i++)
            {
                Card card = myHand[i];
                if (target.getSeed() == card.getSeed())
                {
                    int targetVal = Int16.Parse(target.getVal());
                    int cardVal = Int16.Parse(card.getVal());

                    //gestisco asso e tre
                    if (targetVal == 1) targetVal = 20;
                    if (targetVal == 3) targetVal = 15;

                    if (cardVal == 1) cardVal = 20;
                    if (cardVal == 3) cardVal = 15;

                    if (cardVal > targetVal) return true;
                }
            }
            return false;
        }

        public int checkCarichi()
        {
            int counter = 0;
            for (int i = 0; i < 3; i++)
            {
                Card card = myHand[i];
                if (!card.checkBriscola() && card.getPunteggio() >= 10) counter++;
            }
            return counter;
        }
    }
}

