using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WpfApplication1
{
    public class MyRemote
    {
        public bool _actived = false;
        TcpListener _serverSocket = new TcpListener(IPAddress.Any, 4242);
        Thread _threadServer;
        public Dictionary<string, FuncPtr> _funcTab;

        public void loopTcpRemote()
        {
            this._serverSocket.Start();
            Socket myListenSocket = this._serverSocket.AcceptSocket();
            System.Windows.MessageBox.Show("Remote enabled");
            Byte[] receivedBuffer = new Byte[42];

            while (this._actived == true)
            {
                Array.Clear(receivedBuffer, 0, receivedBuffer.Length);
                int bytesReceived = myListenSocket.Receive(receivedBuffer, receivedBuffer.Length, 0);
                String dataReceived = System.Text.Encoding.ASCII.GetString(receivedBuffer);
                Array.Clear(receivedBuffer, 0, receivedBuffer.Length);
                dataReceived = dataReceived.Replace("\0", String.Empty);
                if (dataReceived.Equals("play") == true || dataReceived.Equals("pause") == true || dataReceived.Equals("stop") == true ||
                    dataReceived.Equals("suivant") == true || dataReceived.Equals("précédent") == true || dataReceived.Equals("fuckoff") == true)
                {
                    if (dataReceived.Equals("fuckoff") == true)
                        this._actived = false;
                    this._funcTab[dataReceived].Invoke();
                }
                else if (dataReceived.Equals(""))
                    this._actived = false;
                dataReceived = "";
            }
            System.Windows.MessageBox.Show("Remote disabled");
        }
    }
}
