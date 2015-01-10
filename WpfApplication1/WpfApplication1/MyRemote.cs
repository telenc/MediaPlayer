using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WpfApplication1
{
        public class MyRemote
        {
            public bool     _actived = false;
            TcpListener     _serverSocket = new TcpListener(IPAddress.Any ,4242);
            Thread          _threadServer; 
            public Dictionary<string, FuncPtr> _funcTab;

            public MyRemote(ref Dictionary<string, FuncPtr> funcTab)
                {
                    this._funcTab = funcTab;
                }
            
            public void loopTcpRemote()
            {
                this._serverSocket.Start();
                Socket myListenSocket = this._serverSocket.AcceptSocket();                               
                System.Windows.MessageBox.Show("Client accepted");
                Byte[] receivedBuffer = new Byte[8];

                while (this._actived == true)
                {
                    int bytesReceived = myListenSocket.Receive(receivedBuffer, receivedBuffer.Length, 0);
                    String dataReceived = System.Text.Encoding.ASCII.GetString(receivedBuffer);
                    dataReceived = dataReceived.Replace("\0", String.Empty);
                    System.Windows.MessageBox.Show("[" + dataReceived + "]");
                    if (dataReceived.Equals("play") == true || dataReceived.Equals("pause") == true || dataReceived.Equals("stop") == true)// ||
                    {
                        this._funcTab[dataReceived].Invoke();
                    }
                    dataReceived.Remove(0);
                }
           }

            public void startRemote()
            {
                this._threadServer = new Thread(new ThreadStart(this.loopTcpRemote));
                this._threadServer.Start();                   
            }
        
            public void stopRemote()
            {
                this._threadServer.Abort();
                this._threadServer.Join();
                System.Windows.MessageBox.Show("Network Thread Stoped");
            }
                
            }   

}
