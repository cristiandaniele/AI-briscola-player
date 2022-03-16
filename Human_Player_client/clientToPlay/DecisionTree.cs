using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clientTCP
{
    class DecisionTree
    {
        //oggetto turn e oggetto match in modo da vedere se posso avanzare in carta
        Match match;
        Turn turn;
        handCard hand;


        public void updateState(Match match, Turn turn, handCard hand)
        {
            this.match = match;
            this.turn = turn;
            this.hand = hand;
        }
        public bool hoLiscio()
        {
            return hand.checkLiscio();
        }

        public bool hoPunti()
        {
            return hand.checkPunti();
        }

        public bool hoBriscola3()
        {
            return hand.checkTre();
        }

        public bool hoBriscola1()
        {
            return hand.checkAsso();
        }

        public bool hoBriscolaBassa()
        {
            return hand.checkBriscolaBassa();
        }

        public bool hoBriscolaMedia()
        {
            return hand.checkBriscolaMedia();
        }

        public bool avanzoInCarta()
        {
            Card onTableCard = turn.getCard();
            return hand.checkAvanzo(onTableCard);
        }

        public bool assoUscito()
        {
            return (match.checkAsso() || hand.checkAsso());
        }

        public bool treUscito()
        {
            return (match.checkTre() || hand.checkTre());
        }

        public bool carichiFiniti()
        {
            int carichi; 
            carichi = match.checkCarichiUsciti();
            carichi += hand.checkCarichi();

            if (carichi == 6) return true;
            return false;
        }

        public bool caricoATerra()
        {
            Card onTableCard = turn.getCard();
            if (onTableCard.getVal() == "1" || onTableCard.getVal() == "3") return true;
            return false;
        }

        public bool puntiATerra()
        {
            Card onTableCard = turn.getCard();
            if (onTableCard.getPunteggio() > 0 && onTableCard.getPunteggio() < 5) return true;
            return false;
        }

        public bool briscolaATerra()
        {
            Card onTableCard = turn.getCard();
            return onTableCard.checkBriscola();
        }

        public bool penultimaMano()
        {
            return match.checkPenultima();
        }

        public int defence_tree()
        {
            int choice = 1;

            if (!hoLiscio())
            {
                choice = 1;
            }

            else
            {
                choice = 0;
                if (!avanzoInCarta())
                {
                    if (!caricoATerra())
                    {
                        if (!puntiATerra())
                        {
                            choice = 0;
                        }
                        else
                        {
                            if (!hoBriscolaBassa())
                            {
                                choice = 0;
                            }
                            else
                            {
                                choice = 1;
                                if (!carichiFiniti())
                                {
                                    choice = 0;
                                    if (!hoBriscolaMedia())
                                    {
                                        if (!penultimaMano())
                                        {
                                            choice = 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        choice = 1;
                        if (!hoBriscolaBassa())
                        {
                            choice = 0;
                            if (hoBriscolaMedia())
                            {
                                choice = 1;
                            }
                        }
                        // choice già 1
                    }
                }
                else
                {
                    choice = 1;
                    if (!treUscito())
                    {
                        if (!briscolaATerra())
                        {
                            choice = 1;
                        }
                        else choice = 0;
                    }
                    else choice = 0;
                }
            }
            return choice;
        }

        public int attack_tree()
        {
            int choice = 0;

            if (!hoLiscio())
            {
                if (!hoPunti())
                {
                    if (!hoBriscolaMedia())
                    {
                        choice = 0;
                        if (!hoBriscolaBassa())
                        {
                            if (!assoUscito())
                            {
                                choice = 1;
                            }
                            else
                            {
                                if (!hoBriscola3())
                                {
                                    choice = 0;
                                    if (!hoBriscola1())
                                    {
                                        choice = 1;
                                    }
                                    else choice = 0;
                                }
                                else choice = 0;
                            }
                        }
                        else
                        {
                            if (!treUscito())
                            {
                                choice = 0;
                                if (!assoUscito())
                                {
                                    choice = 1;
                                }
                                else choice = 0;
                            }
                            else choice = 0;
                        }
                    }
                    else choice = 0;
                }
                else
                {
                    choice = 1;
                    if (!hoBriscolaBassa())
                        choice = 1;
                    else
                    {
                        if (!assoUscito())
                        {
                            choice = 0;
                            if (!treUscito())
                            {
                                choice = 0;
                                if (!hoBriscolaMedia())
                                {
                                    choice = 0;
                                }
                                else choice = 1;
                            }
                            else
                            {
                                if (!hoBriscolaMedia())
                                {
                                    choice = 1;
                                }
                                else choice = 0;
                            }
                        }
                        else choice = 1;
                    }
                }
            }
            return choice;
        }
    }
}
