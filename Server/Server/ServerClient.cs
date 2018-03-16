using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;

namespace Server
{
    class ServerClient
    {
        /// <summary>
        /// Reader des Clients
        /// </summary>
        public StreamReader Reader { get; private set; }

        /// <summary>
        /// Writer des Clients
        /// </summary>
        public StreamWriter Writer { get; private set; }

        /// <summary>
        /// Network Client
        /// </summary>
        public TcpClient Client { get; private set; }

        /// <summary>
        /// Name des Clients
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Thread welcher auf Data wartet
        /// </summary>
        public Thread ReceiveThread { get; private set; }

        private Server server;

        public ServerClient(Server server, TcpClient client)
        {
            this.server = server;
            Client = client;
            Reader = new StreamReader(client.GetStream());
            Writer = new StreamWriter(client.GetStream());
            ReceiveThread = new Thread(ReceiveData);
            ReceiveThread.IsBackground = true;
            ReceiveThread.Start();
        }

        /// <summary>
        /// Wartet auf Daten und bearbeitet diese
        /// </summary>
        private void ReceiveData()
        {
            try
            {
                while (true)
                {
                    string receivedData = null;
                    while ((receivedData = Reader.ReadLine()) != null)
                    {
                        Command command = Command.ToCommand(receivedData);
                        server.HandleData(this, command);
                    }
                }
            }
            catch
            {
                server.HandleDisconnect(this);
            }
        }

        /// <summary>
        /// Sendet Daten an den Client
        /// </summary>
        public void Send(Command command)
        {
            Writer.WriteLine(command.ToString());
            Writer.Flush();
        }

        /// <summary>
        /// Stoppt den Client und räumt auf
        /// </summary>
        public void Close()
        {
            Client.GetStream().Close();
            Client.Close();
            ReceiveThread.Interrupt();
            Reader.Close();
            Writer.Close();
        }
    }
}
