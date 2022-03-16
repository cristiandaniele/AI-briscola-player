using System;
using System.Security.Cryptography;
using System.Text;

namespace clientTCP
{
    public class Card
    {
        string valore;
        string seme;
        int random;
        public bool valid;
        int punteggio;
        bool isBriscolaAterra;
        int pow;
        bool briscola;
        public Card(string valore, string seme, int rand, bool valid,int punteggio,int pow,string briscolaSeed)
        {
            this.valore = valore;
            this.seme = seme;
            random = rand;
            this.valid = valid;
            this.punteggio = punteggio;
            isBriscolaAterra = false;
            this.pow = pow;
            if (briscolaSeed == seme)
            {
                briscola = true;
            }
            if (random == -1)
            {
                isBriscolaAterra = true;
            }
        }

        public bool checkBriscola()
        {
            return briscola;
        }
        public int getPunteggio()

        {

            return punteggio;

        }

        public bool getIsBriscola()

        {

            return isBriscolaAterra;

        }

        public int getPow()
        {
            return pow;
        }

        public void printCard()
        {
            if (isBriscolaAterra == true)
            {
                Console.WriteLine("[GAME]La briscola è: ");
            }

            Console.WriteLine("[GAME]Valore: " + valore + " Seme: " + seme + " Random: " + random + " Valid: " + valid + " Points: " + punteggio+ " Pow: "+pow);
        }

        public int getRandom()
        {
            return random;
        }
        public string getVal()
        {
            return valore;
        }
        public string getSeed()
        {
            return seme;
        }
        public static string GetString(byte[] bytes)
        {
            bool even = (bytes.Length % 2 == 0);
            char[] chars = new char[1 + bytes.Length / sizeof(char) + (even ? 0 : 1)];
            chars[0] = (even ? '0' : '1');
            System.Buffer.BlockCopy(bytes, 0, chars, 2, bytes.Length);

            return new string(chars);
        }
        public string calculateHash()
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                if (random != -1)
                {/*
                    Console.WriteLine("Calcolo l'hash di " + valore + " + " + seme + " + " + random);
                    string tmp = valore + seme + random;
                    byte[] bytes = Encoding.ASCII.GetBytes(tmp);
                    string hex = BitConverter.ToString(bytes);
                    Console.WriteLine("DEBUG" + hex);
                    byte[] hashValue = mySHA256.ComputeHash(bytes);
                    Console.WriteLine(hashValue);
                    string hash = Convert.ToBase64String(hashValue, 0, hashValue.Length);
                    Console.WriteLine(hash);
                    return hash;
                    */
                    return (valore + seme + random);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR-GAME]Cannot calcolate this hash card:");
                    printCard();
                    return "-1";
                }
            }
        }
    }
}
