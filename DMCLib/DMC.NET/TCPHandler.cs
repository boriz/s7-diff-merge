using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DMCBase;

namespace DMC.Net
{
    public class TCPHandler
    {
        public event EventHandler<EventArgs<TCPMessage>> TCPMessageReceived;

        public bool Connected
        {
            get { return this.tcpClient.Client.Connected; }
        }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private TcpClient tcpClient;
        private Thread thread;
        private bool listening;


        public TCPHandler(TcpClient client)
        {
            this.tcpClient = client;

            this.listening = true;
            this.thread = new Thread(Run);
            this.thread.Start();
        }

        public void Stop()
        {
            this.listening = false;
            this.tcpClient.GetStream().Close();
            this.tcpClient.Close();

            this.thread.Join();
        }

        private void Run()
        {
            NetworkStream stream = this.tcpClient.GetStream();

            try
            {
                int numBytesRead;

                while (this.listening)
                {
                    int messageLength = stream.ReadByte();

                    if (messageLength == -1)
                    {
                        break;
                    }

                    byte[] messageBuffer = new byte[messageLength];

                    numBytesRead = stream.Read(messageBuffer, 0, messageBuffer.Length);

                    if (numBytesRead != messageLength)
                    {
                        break;
                    }

                    string messageName = Encoding.ASCII.GetString(messageBuffer, 0, messageBuffer.Length);

                    byte[] dataLengthBuffer = new byte[4];
                    
                    stream.Read(dataLengthBuffer, 0, dataLengthBuffer.Length);

                    int dataLength = dataLengthBuffer.ToInt32(0);

                    byte[] dataBuffer = new byte[dataLength];

                    int index = 0;
                    while ((index < dataBuffer.Length) && this.listening)
                    {
                        int numBytesRemaining = dataBuffer.Length - index;

                        numBytesRead = stream.Read(dataBuffer, index, numBytesRemaining);

                        if (numBytesRead == 0)
                        {
                            break;
                        }

                        index += numBytesRead;
                    }

                    if (index != dataLength)
                    {
                        break;
                    }

                    object data = Encoding.ASCII.GetString(dataBuffer);
                    
                    logger.Debug("TCP Message Received: {0}", messageName);

                    if (this.TCPMessageReceived != null)
                    {
                        this.TCPMessageReceived(
                            this,
                            new EventArgs<TCPMessage>(
                                new TCPMessage()
                                {
                                    Data = data,
                                    Name = messageName,
                                }));
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorException(e.Message, e);
            }
            finally
            {
                stream.Close();
                this.tcpClient.Close();

                logger.Debug("TCP Handler Closed");
            }
        }
    }
}
