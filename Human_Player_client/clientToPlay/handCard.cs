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
            List<Card> hand = getAllCards();

            hand.RemoveAll(x => x == toRemove);
            myHand = hand.ToArray();
        }

        public void insertCard(Card x)
        {
            List<Card> hand = getAllCards();
            hand.Add(x);
            myHand = hand.ToArray();
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
            Card toInsert = new Card((val), seed, Convert.ToInt32(random), true, Convert.ToInt32(points), Convert.ToInt32(pow), myHand1.getBriscolaSeed());
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Pesco:");
            toInsert.printCard();
            insertCard(toInsert);

        }
        public void scarta(int index)
        {
            //to test
            myHand = myHand.Where((e, i) => i != index).ToArray();
        }
        public void printHand()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("In mano ho le seguenti carte:");

            List<Card> hand = getAllCards();

            hand.ForEach(x => x.printCard());
            briscola.printCard();
        }
        public Card getCard(int index)
        {
            return myHand[index];
        }

        public bool checkLiscio()
        {
            List<Card> hand = getAllCards();

            Card res = hand.Find(x => !x.checkBriscola() && x.getPunteggio() == 0);

            if (res != null) return true;
            return false;
        }

        public bool checkPunti()
        {
            List<Card> hand = getAllCards();

            Card res = hand.Find(x => !x.checkBriscola() && x.getPunteggio() > 0 && x.getPunteggio() < 10);

            if (res != null) return true;
            return false;
        }

        public bool checkTre()
        {
            List<Card> hand = getAllCards();

            Card res = hand.Find(x => x.checkBriscola() && x.getPunteggio() == 10);

            if (res != null) return true;
            return false;
        }

        public bool checkAsso()
        {
            List<Card> hand = getAllCards();

            Card res = hand.Find(x => x.checkBriscola() && x.getPunteggio() == 11);

            if (res != null) return true;
            return false;
        }

        public bool checkBriscolaBassa()
        {
            List<Card> hand = getAllCards();

            Card res = hand.Find(card => card.checkBriscola() && (card.getVal() == "2" || card.getVal() == "4" || card.getVal() == "5" || card.getVal() == "6"));

            if (res != null) return true;
            return false;
        }

        public bool checkBriscolaMedia()
        {
            List<Card> hand = getAllCards();

            Card res = hand.Find(card => card.checkBriscola() && (card.getVal() == "7" || card.getVal() == "8" || card.getVal() == "9" || card.getVal() == "10"));

            if (res != null) return true;
            return false;
        }

        public bool checkAvanzo(Card target)
        {
            if (target.checkBriscola()) return false; //solo avanzo su non briscole?

            List<Card> hand = getAllCards();

            Card res = hand.Find(card => card.getSeed()==target.getSeed() && card.getPow()>target.getPow());

            if (res != null) return true;
            return false;
        }

        public int checkCarichi()
        {
            List<Card> hand = getAllCards();

            List<Card> listCarichi = hand.FindAll(card => !card.checkBriscola() && card.getPunteggio() >= 10);

            int res = listCarichi.Count;

            return res;
        }
    }
}

