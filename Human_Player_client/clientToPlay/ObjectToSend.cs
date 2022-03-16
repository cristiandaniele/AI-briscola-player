using System;
namespace clientTCP
{
    public class ObjectToSend
    {
        string header = "";
        string payload = "";
        public ObjectToSend(string h, string p)
        {
            header = h;
            payload = p;
        }
        public ObjectToSend(string h, int p)
        {
            header = h;
            payload = p.ToString();
        }

        public void setHeader(string h)
        {
            header = h;
        }
        public void setPayload(string p)
        {
            payload = p;
        }

        public string getFormattedData()
        {
            string formattedString = "-1";
            if (header == "" || payload == "")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Missing argouments");

            }
            else
            {
                formattedString = "{header:"+header+",Payload:"+payload+"}";
            }
            return formattedString;
        }


    }
}
