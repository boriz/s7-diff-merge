using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DMC.IO;
using DMCBase;

namespace DMC.Net
{
    public class TCPMessageClient
    {
        public string RemoteHost { get; set; }

        public TCPMessageClient(string remoteHost)
        {
            this.RemoteHost = remoteHost;
        }

        public void SendMessage(TCPMessage message)
        {
            TcpClient tcpClientSend = null;
            NetworkStream stream = null;
            Exception exception = null;

            string iniString = message.Data.ToINIString();

            byte[] buffer = new byte[message.Name.Length + iniString.Length + 5];

            buffer[0] = (byte)message.Name.Length;
            Array.Copy(Encoding.ASCII.GetBytes(message.Name), 0, buffer, 1, message.Name.Length);
            Array.Copy(iniString.Length.ToByteArray(), 0, buffer, 1 + message.Name.Length, 4);
            Array.Copy(Encoding.ASCII.GetBytes(iniString), 0, buffer, 1 + message.Name.Length + 4, iniString.Length);

            try
            {
                tcpClientSend = new TcpClient(this.RemoteHost, TCPMessageServer.ListeningPort)
                {
                    SendTimeout = 1000,
                };

                stream = tcpClientSend.GetStream();
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                if (tcpClientSend != null)
                {
                    stream.Close();
                    tcpClientSend.Close();
                }
            }

            if (exception != null)
            {
                throw exception;
            }
        }

        public void Close()
        {
        }
    }
}
