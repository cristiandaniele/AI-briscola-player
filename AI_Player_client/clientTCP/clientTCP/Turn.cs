using System;
namespace clientTCP
{
    public class Turn
    {
        bool yourTurn;
        Card onTableCard;
        bool valid;
        //carico a terra
        public Turn()
        {
            yourTurn = false;
            onTableCard = null;
            valid = false;
        }

        public bool getValid()
        {
            return valid;
        }
        public void validate()
        {
            valid = true;
        }
        public void setTurn(bool turn)
        {
            yourTurn = turn;
        }
        public void setCard(Card x)
        {
            onTableCard = x;
        }
        public void printTurn()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[GAME]");
            if (yourTurn == true)
            {
                Console.WriteLine("E' il tuo turno");
            }
            else
            {
                Console.WriteLine("E' il turno dell'avversario");
            }
            if (onTableCard!=null)
            {
                Console.WriteLine("[Game]Devi difenderti da questa carta!");
                onTableCard.printCard();
            }
            else
            {
                Console.WriteLine("[Game]Nessuna carta sul tavolo");
            }

        }
        public Card getCard()
        {
            return onTableCard;
        }
    }
}
