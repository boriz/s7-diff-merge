using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DMC.IO;
using DMCBase;

namespace DMC.Net
{
    public class TCPMessageServer
    {
        public const int DiscoveryPort = 7777;
        public const int ListeningPort = 8888;
        public const int BroadcastDelay = 1000;

        public bool IsConnected { get; private set; }
        
        public string Name { get; set; }
        public Dictionary<string, Type> MessageDataTypes { get; set; }

        public event EventHandler<EventArgs<TCPMessage>> TCPMessageReceived;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Thread threadTCPDaemon;
        private Thread threadRecliaming;
        private Thread threadUDPBroadcast;
        private Thread threadUDPListener;
        private bool listening;
        private bool recliaming;
        private TcpListener tcpListener;
        private ArrayList tcpHandlers;

        public TCPMessageServer(string name, Dictionary<string, Type> messageDataTypes)
        {
            this.Name = name;
            this.MessageDataTypes = messageDataTypes;

            this.IsConnected = false;

            this.tcpHandlers = new ArrayList();

            this.listening = true;
            this.threadTCPDaemon = new Thread(RunTCPDaemon);
            this.threadTCPDaemon.Start();

            this.recliaming = true;
            this.threadRecliaming = new Thread(CloseInactiveHandlers);
            this.threadRecliaming.Start();

            this.threadUDPBroadcast = new Thread(UDPBroadcast);
            this.threadUDPBroadcast.Start();

            this.threadUDPListener = new Thread(UDPListen);
            this.threadUDPListener.Start();
        }

        public void Stop()
        {
            this.listening = false;
            if (this.tcpListener != null)
            {
                this.tcpListener.Stop();
            }

            this.threadTCPDaemon.Join();

            this.recliaming = false;
            this.threadRecliaming.Join();

            foreach (TCPHandler tcpHandler in this.tcpHandlers)
            {
                tcpHandler.Stop();
            }
        }

        private void RunTCPDaemon()
        {
            while (this.listening)
            {
                try
                {
                    if (this.tcpListener == null)
                    {
                        this.tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, ListeningPort));
                        this.tcpListener.Start();
                    }

                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    TCPHandler tcpHandler = new TCPHandler(tcpClient);
                    tcpHandler.TCPMessageReceived += new EventHandler<EventArgs<TCPMessage>>(TCPMessageReceivedCallback);

                    lock (this.tcpHandlers.SyncRoot)
                    {
                        this.tcpHandlers.Add(tcpHandler);
                    }

                    logger.Debug("TCP Handler Added: {0}", tcpClient.Client.RemoteEndPoint as IPEndPoint);
                }
                catch (Exception ex)
                {
                    logger.ErrorException("Exception on Run TCP Daemon", ex);
                }
            }

            if (this.tcpListener != null)
            {
                this.tcpListener.Stop();
            }
        }

        private void CloseInactiveHandlers()
        {
            while (this.recliaming)
            {
                lock (this.tcpHandlers.SyncRoot)
                {
                    for (int i = this.tcpHandlers.Count - 1; i >= 0; i--)
                    {
                        object tcpHandler = this.tcpHandlers[i];
                        if (!((TCPHandler)tcpHandler).Connected)
                        {
                            this.tcpHandlers.Remove(tcpHandler);
                            logger.Debug("TCP Handler Removed");
                        }
                    }
                }

                Thread.Sleep(200);
            }
        }

        private void UDPBroadcast()
        {
            UdpClient client = new UdpClient()
            {
                EnableBroadcast = true,
            };

            byte[] buffer = Encoding.ASCII.GetBytes(this.Name);

            while (this.listening)
            {
                client.Send(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, DiscoveryPort));

                Thread.Sleep(BroadcastDelay);
            }
        }

        private void UDPListen()
        {
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, DiscoveryPort))
            {
                EnableBroadcast = true,
            };

            while (this.listening)
            {
            }
        }

        private void TCPMessageReceivedCallback(object sender, EventArgs<TCPMessage> e)
        {
            if ((this.TCPMessageReceived != null) &&
                this.listening &&
                this.recliaming)
            {
                this.TCPMessageReceived(sender, e);
            }
        }

        private object DeserializeFromINIString(string iniString, string messageName)
        {
            if (this.MessageDataTypes.ContainsKey(messageName))
            {
                Type dataType = this.MessageDataTypes[messageName];

                return iniString.FromINIString(dataType);
            }

            return null;
        }
    }
}
