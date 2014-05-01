using System;
using System.IO.Ports;
using System.Diagnostics;
using System.Windows;

namespace Windows_Trend_View
{
    /// <summary>
    /// Class to communicate to PLC over Profinet or PROFIBUS
    /// </summary>
    public abstract class PLCEngine : NotifyPropertyChangedBase
    {
        private static libnodave.daveOSserialType fds;
        private static libnodave.daveInterface PLCInterface;
        private static libnodave.daveConnection PLCConnection;

        //  protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// State of PLC communications (Connected, Disconnected, etc.)
        /// </summary>
        private PLCState state;

        public PLCState State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    Debug.WriteLine("New State: " + value.ToString());
                    this.state = value;
                    NotifyPropertyChanged("State");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the MCUEngine class.
        /// </summary>
        public PLCEngine()
        {
            // Start in the Disconnected state
            this.State = PLCState.Disconnected;
        }

        /// <summary>
        /// Connect to the MCU.
        /// </summary>
        /// <param name="comPort">Com port over which to connect to the MCU.</param>
        public virtual bool Connect(string Address, int rack, int slot)
        {
            rack = 0;
            slot = 2;

            // We're connecting...
            this.State = PLCState.Connecting;

            try
            {
                // Open the specified port
                fds.rfd = libnodave.openSocket(102, Address);
                fds.wfd = fds.rfd;
                //If connection successful
                if (fds.rfd > 0)
                {
                    PLCInterface = new libnodave.daveInterface(fds, "IF1", 0, libnodave.daveProtoISOTCP,
                        libnodave.daveSpeed187k);
                    PLCInterface.setTimeout(1000000);
                    //	    res=di.initAdapter();	// does nothing in ISO_TCP. But call it to keep your programs indpendent of protocols
                    PLCConnection = new libnodave.daveConnection(PLCInterface, 0, rack, slot);
                    if (0 == PLCConnection.connectPLC())
                    {
                        // Now we're connected!
                        this.State = PLCState.Connected;
                    }
                }
            }
            catch
                (Exception ex)
            {
                MessageBox.Show("Exception Connecting to Port!\r\n\r\n" + ex.Message);
                //  logger.ErrorException("Exception Connecting to Port", ex);
                this.State = PLCState.Disconnected;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Disconnects from the PLC by closing the port
        /// </summary>
        public virtual void Disconnect()
        {

            if (this.State != PLCState.Disconnected)
            {
                // Now we're disconnecting
                this.State = PLCState.Disconnecting;

                PLCConnection.disconnectPLC();
                //	    di.disconnectAdapter();	// does nothing in ISO_TCP. But call it to keep your programs indpendent of protocols
                libnodave.closeSocket(fds.rfd);
                // Now we're disconnected
                this.State = PLCState.Disconnected;
            }
        }


        /// <summary>
        /// Sends a read or write command to the library
        /// </summary>
        /// <param name="command">command to send
        public bool Send(int Area, int dbNumber, int baseAddress, int Length)
        {
            //Commands here
            int res;
            res = PLCConnection.readBytes(libnodave.daveDB, dbNumber, baseAddress, Length, null);

            return false;
        }

        //Read bytes from the PLC
        public bool ReadBytes(AREA_TYPE Area, int dbNumber, int baseAddress, int Length)
        {
            int res;
            res = PLCConnection.readBytes((int) Area, dbNumber, baseAddress, Length, null);

            if (res == 0)
            {
                return true;
            }
            else
                return false;
        }
    }
}