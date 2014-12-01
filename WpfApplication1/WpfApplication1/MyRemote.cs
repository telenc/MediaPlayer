using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            
            public void loopTcpRemote()
            {
                this._serverSocket.Start();
                Socket myListenSocket = this._serverSocket.AcceptSocket();                               
                System.Windows.MessageBox.Show("Client Resquest accepted, socket init");

                while (this._actived == true)
                {
                    Byte[] receivedBuffer = new Byte[1024];
                    int bytesReceived = myListenSocket.Receive(receivedBuffer, receivedBuffer.Length, 0);
                    String dataReceived = System.Text.Encoding.ASCII.GetString(receivedBuffer);
                    System.Windows.MessageBox.Show("[" + dataReceived + "]");
                    System.Threading.Thread.Sleep(1000);
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
