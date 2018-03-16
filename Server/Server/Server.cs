using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Server
    {
        /// <summary>
        /// Alle Nachrichten die empfangen worden sind
        /// </summary>
        public Dictionary<string, List<Tuple<string, string>>> ReceivedMessages { get; private set; }

        /// <summary>
        /// Alle verbundenen Clients
        /// </summary>
        public List<ServerClient> Clients { get; private set; }

        /// <summary>
        /// EndPoint des Servers
        /// </summary>
        public IPEndPoint EndPoint { get; private set; }

        /// <summary>
        /// TcpListener zum Warten auf Anfragen
        /// </summary>
        public TcpListener Listener { get; private set; }

        /// <summary>
        /// Thread zum Annehmen der Anfragen
        /// </summary>
        public Thread AcceptThread { get; private set; }

        public Server()
        {
            ReceivedMessages = new Dictionary<string, List<Tuple<string, string>>>();
            EndPoint = new IPEndPoint(IPAddress.Loopback, 1025);
            Clients = new List<ServerClient>();
            Listener = new TcpListener(EndPoint);
            Listener.Start();
            AcceptThread = new Thread(Accept);
            AcceptThread.IsBackground = true;
            AcceptThread.Start();
        }

        /// <summary>
        /// Nimmt Verbindungsanfragen an, sofern der Server nicht voll ist
        /// </summary>
        private void Accept()
        {
            while (true)
            {
                if (Clients.Count < 10)
                {
                    TcpClient client = Listener.AcceptTcpClient();
                    Clients.Add(new ServerClient(this, client));
                    Console.WriteLine("Client angenommen! IP: " + client.Client.LocalEndPoint);
                }
            }
        }

        /// <summary>
        /// Verarbeitet die erhalten Daten von den Clients
        /// </summary>
        public void HandleData(ServerClient client, Command command)
        {
            Console.WriteLine("Daten erhalten! ID: " + command.Identifier);
            if(command.Identifier == "sendMessage")
            {
                List<Tuple<string, string>> messageDatas = new List<Tuple<string, string>>();
                if (ReceivedMessages.ContainsKey(client.Name))
                {
                    messageDatas = ReceivedMessages[client.Name];
                    ReceivedMessages.Remove(client.Name);
                }
                messageDatas.Add(new Tuple<string, string>(command.Arguments[0], command.Arguments[1]));
                ReceivedMessages.Add(client.Name, messageDatas);
                ServerClient receiver = GetClientByName(command.Arguments[0]);
                if(receiver != null)
                {
                    receiver.Send(new Command("sendText", client.Name, command.Arguments[1]));
                }
            }
            else if(command.Identifier == "sendName")
            {
                client.Name = command.Arguments[0];
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn ein Client den Server verlässt
        /// </summary>
        public void HandleDisconnect(ServerClient client)
        {
            Clients.Remove(client);
            Console.WriteLine("Client hat den Server verlassen! Name: " + client.Name);
        }

        /// <summary>
        /// Gibt das ServerClient Objekt zum gegebenen Namen zurück
        /// </summary>
        private ServerClient GetClientByName(string name)
        {
            foreach(ServerClient client in Clients)
            {
                if(client.Name == name)
                {
                    return client;
                }
            }
            return null;
        }
    }
}
