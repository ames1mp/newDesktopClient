using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop_Client
{
    class MessageStore
    {
        public List<String> messages { get; set; }

        public MessageStore()
        {
            messages = new List<String>();
        }

        public static void writeInfo(string message) {

            System.IO.File.WriteAllText(@"C:\Users\Mike\Desktop\WriteLines.txt", message);
        }
    }
}
