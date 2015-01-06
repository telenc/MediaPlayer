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
            private System.Windows.Controls.MediaElement mediaElement1;
           
            public    MyRemote(ref System.Windows.Controls.MediaElement mediaElement1)
                {
                    this.mediaElement1 = mediaElement1;
                }
            
            public void loopTcpRemote()
            {
                this._serverSocket.Start();
                Socket myListenSocket = this._serverSocket.AcceptSocket();                               
                System.Windows.MessageBox.Show("Client Resquest accepted, socket init");
                Byte[] receivedBuffer = new Byte[8];

                while (this._actived == true)
                {
                    int bytesReceived = myListenSocket.Receive(receivedBuffer, receivedBuffer.Length, 0);
                    String dataReceived = System.Text.Encoding.ASCII.GetString(receivedBuffer);
                    dataReceived = dataReceived.Replace("\0", String.Empty);
                    System.Windows.MessageBox.Show("[" + dataReceived + "]");
                    if (dataReceived.Equals("play") == true) {
                        this.mediaElement1.Play();
                        this.mediaElement1.SpeedRatio = 1; }
                    else if (dataReceived.Equals("stop") == true)
                    {
                        this.mediaElement1.Stop();
                        this.mediaElement1.Close();
                    }
                    else if (dataReceived.Equals("pause") == true)
                    {
                        this.mediaElement1.Pause();
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
