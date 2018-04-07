﻿
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop_Client
{
    public class StateObject
    {
        //Client socket.
        public Socket workSocket = null;

        //Size of the receive buffer;
        public const int BufferSize = 4096;

        //Receive buffer
        public byte[] buffer = new byte[BufferSize];

        //Received data string;
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchSocketListener
    {
        public const int port = 8417;

        //Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchSocketListener()
        {


        }

        public static void StartListening()
        {
            ConnectionViewModel connectionViewModel = new ConnectionViewModel();


            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress myIp in ipHostInfo.AddressList)
            {
                Console.WriteLine(string.Format("myIp: {0} myFam: {1}", myIp, myIp.AddressFamily));
            }
            IPAddress ipAddress = ipHostInfo.AddressList[2];
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 8417);
            Console.WriteLine(string.Format("My IP ADDRESS: {0}", localEndPoint));
            Console.WriteLine(string.Format("My IP ADDRESS: {0}", ipAddress));

            TcpListener serverSocket = new TcpListener(localEndPoint);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            Console.WriteLine(" >> " + "Server Started");


            handleClient client1 = new handleClient();
            client1.startClient(clientSocket, Convert.ToString(counter));





            App.Current.Dispatcher.Invoke(() => {
                Label conStatusBox = (Label)App.Current.MainWindow.FindName("conStatusBox");
                conStatusBox.Content = " >> " + "Waiting for a connection.";
            });
           // MessageStore.writeInfo("Waiting for a connection.");
            connectionViewModel.ConnectionStatus = "Waiting for a connection";

            counter = 0;
            while (true)
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started!");
                //App.Current.Dispatcher.Invoke(() => {
                //    Label conStatusBox = (Label)App.Current.MainWindow.FindName("conStatusBox");
                //    conStatusBox.Content = " >> " + "Client No:" + Convert.ToString(counter) + " started!";
                //});

                handleClient client = new handleClient();
                client.startClient(clientSocket, Convert.ToString(counter));
            }

            //clientSocket.Close();
           // serverSocket.Stop();
            //Console.WriteLine(" >> " + "exit");
         //   Console.ReadLine();
        }

    }

    //Class to handle each client request separatly
    public class handleClient
    {

        MessageStore mstore;
        TcpClient clientSocket;
        string clNo;
        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            this.mstore = new MessageStore();
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            Thread ctThread = new Thread(DoChat);
            ctThread.Start();
        }
        private void DoChat()
        {

            MessageStore.writeInfo("Test");

            int requestCount = 0;
            Boolean connected = true;

            while ((connected))
            {
                try
                {
                    if (clientSocket.Connected)
                    {
                        requestCount = requestCount + 1;
                        NetworkStream networkStream = clientSocket.GetStream();
                        byte[] myReadBuffer = new byte[1024];
                        StringBuilder myCompleteMessage = new StringBuilder();
                        int numberOfBytesRead = 0;
                        do
                        {
                            numberOfBytesRead = networkStream.Read(myReadBuffer, 0, myReadBuffer.Length);
                            myCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
                        }
                        while (networkStream.DataAvailable);
                        if (numberOfBytesRead > 0)
                        {
                            string temp = myCompleteMessage.ToString();
                            Console.WriteLine(" >> " + "From client-" + temp + " " + DateTime.Now);
                            mstore.messages.Add(temp);
                         
                            string[] myCommand = temp.Split(' ');
                            if (myCommand[0].Contains("Connect"))
                            {
                                Console.WriteLine("Client Connected " + clientSocket.Client.RemoteEndPoint + " " + DateTime.Now);
                                string serverResponse = "404 OK\r\n";
                                byte[] sendBytes = Encoding.UTF8.GetBytes(serverResponse);
                                networkStream.Write(sendBytes, 0, sendBytes.Length);
                            }
                            else if (myCommand[0].Contains("Disconnect"))
                            {
                                connected = false;
                                Console.Write("Client D/C " + clientSocket.Client.RemoteEndPoint + " " + DateTime.Now);
                            }
                            //networkStream.Flush();

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
                }
            }
        }
    }




    class Server
    {
        
       
    }
}
