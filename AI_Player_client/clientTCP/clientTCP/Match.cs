using System;

using System.Collections.Generic;
namespace clientTCP
{
    public class Match
    {
        List<Card> expiredCards = new List<Card>();
        int punti;
        //funzione che dice se sono usciti 3, asso, se sono finiti i carichi, se è la penultima mano
        public Match()
        {
            punti = 0;
        }
        public void addCard(Card x)
        {
            expiredCards.Add(x);
        }
        public void updatePoints(int points)
        {
            punti += points;
        }
        public bool canPlay()
        {
            if (expiredCards.Count>=40)
            {
                return false;
            }
            return true;
        }
        public int getPoints()
        {
            return punti;
        }

        public int checkCaricoSeed(Card x)
        {
            int cont = 0;
            foreach(Card y in expiredCards)
            {
                if (x.getSeed()==y.getSeed()&& y.getPunteggio()>=2) cont+= 1;
            }
            return cont;
        }

        public void printExpiredCards()
        {
            //Console.ForegroundColor = ConsoleColor.Cyan;
            //Console.WriteLine("Carte uscite:");
            //foreach (Card x in expiredCards)
            //{
            //    x.printCard();
            //}
        }

        public bool checkAsso()
        {
            Card c = expiredCards.Find(x => x.getVal() == "1");
            if (c != null) return true;
            return false;
        }

        public bool checkTre()
        {
            Card c = expiredCards.Find(x => x.getVal() == "3");
            if (c != null) return true;
            return false;
        }

        public bool checkPenultima()
        {
            //dovrebbero essere 32 ma cristian è scarso e sono 31
            if (expiredCards.Count == 31) return true;
            return false;
        }

        public int checkCarichiUsciti()
        {
            List<Card> carichi = expiredCards.FindAll(x => x.getPunteggio() >= 10 && !x.checkBriscola());
            return carichi.Count;
        }
    }
}
