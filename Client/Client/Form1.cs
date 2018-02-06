using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    public partial class Form1 : Form
    {
        public string Name { get; set; }
        public string Password { get; set; }
        private IPEndPoint myEndPoint;
        private Socket mySocket;
        private int port;
        private IPAddress myIP;
        public Form1()
        {
            InitializeComponent();
            myIP = IPAddress.Parse("localhost");
            port = 8888;
            myEndPoint = new IPEndPoint(myIP, port);
        }

        void Connectionload()   //In eine Methode gepackt, damit es nicht einfach im Komstruktur rumfliegt
        {
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                mySocket.Connect(new IPEndPoint(IPAddress.Parse("localhost"), 8887)); // Die Ports könnten auch gleich sein müssen sie aber nicht

            }
            catch (Exception)
            {

                throw;
            }

            if (mySocket.Connected)
            {
                MessageBox.Show("Connection established.");
                mySocket.Close();
            }

    }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
