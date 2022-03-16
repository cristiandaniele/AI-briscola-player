using System;

using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
namespace clientTCP
{
    public class WrapperTCP
    {
        static string HOST = null;
        static int PORT = 0;
        static TcpClient client;
        NetworkStream nwStream;

        public WrapperTCP(string host, int port)
        {
            HOST = host;
            PORT = port;
        }

        public void StartConnection()
        {

            if (client != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("[CONNECTION-WARNING]Connection is already established");
            }
            else
            {
                try
                {
                    client = new TcpClient();
                    client.Connect(HOST, PORT);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[CONNECTION]Connection established");
                }
                catch (Exception e)
                {
                    client = null;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[CONNECTION-ERROR]Cannot establish connection");
                }
            }
        }
        public void SendData(string data)
        {
            nwStream = client.GetStream();
            byte[] byteToSend = Encoding.ASCII.GetBytes(data);
            nwStream.Write(byteToSend, 0, byteToSend.Length);

        }
        public void SendData(object data)
        {
            nwStream = client.GetStream();
            byte[] byteToSend = Encoding.ASCII.GetBytes(data.ToString());
            nwStream.Write(byteToSend, 0, byteToSend.Length);

        }
        public string ReceiveData(bool print)
        {
           
            nwStream = client.GetStream();
            byte[] byteToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(byteToRead, 0, client.ReceiveBufferSize);
            string read;
            read = Encoding.ASCII.GetString(byteToRead, 0, bytesRead);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            return read;
        }
        public string ReceiveData()
        {


            nwStream = client.GetStream();
            byte[] byteToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(byteToRead, 0, client.ReceiveBufferSize);
            string read;
            read = Encoding.ASCII.GetString(byteToRead, 0, bytesRead);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            return read;
        }

        static void CloseConnection()
        {
            if (client == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("[CONNECTION-WARNING] Connection already closed");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[CONNECTION]Connection closed");
                client.Close();
                client = null;
            }
        }

    }
}
