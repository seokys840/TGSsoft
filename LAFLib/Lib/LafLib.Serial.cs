using System;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace LAFLib
{
    public partial class LAF
    {
        ASCIIEncoding asc = new ASCIIEncoding();
        //------------------ Serial ------------------------------------
        public void Open(int _comport, int _BaudRate = 9600)
        {
            try
            {
                CloseTransport();

                isLan = false;
                comport = _comport;
                bRate = _BaudRate;
                if (_comport < 10)
                    xsp = new SerialPort("COM" + comport.ToString());
                else
                {
                    string strTemp;
                    strTemp = "\\\\.\\COM";
                    strTemp += comport;
                    xsp = new SerialPort("COM" + strTemp.ToString());
                }

                

                xsp.BaudRate = _BaudRate;                
                xsp.Parity = Parity.None;                
                xsp.DataBits = 8;
                xsp.StopBits = StopBits.One;

                xsp.ReadTimeout = 1000;
                xsp.Handshake = Handshake.None;

                xsp.Open();
                port = new SerialTransport(xsp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// LAN(TCP) 으로 연결한다. 시리얼과 동일한 텍스트 프로토콜(";uc ...\r\n")을 사용한다.
        /// </summary>
        /// <param name="ip">장비 IP 주소 (호스트명도 가능)</param>
        /// <param name="tcpPort">장비 TCP 포트</param>
        public void OpenLan(string ip, int tcpPort)
        {
            CloseTransport();

            port = new TcpTransport(ip, tcpPort, 3000);
            port.ReadTimeout = 1000;

            isLan = true;
            ipAddress = ip;
            this.tcpPort = tcpPort;
        }

        public void Close()
        {
            //CloseTransceiver();
            CloseTransport();
        }

        void CloseTransport()
        {
            if (port != null)
            {
                port.Close();
                port = null;
            }
            else if (xsp.IsOpen)
                xsp.Close();
        }

        ITransport Port
        {
            get
            {
                if (port == null)
                    throw new InvalidOperationException("The port is not open.");
                return port;
            }
        }

        void DiscardInBuffer()
        {
            Port.DiscardInBuffer();
        }

        public void WritePort(string command)
        {
            command += "\r\n";
            byte[] ba = asc.GetBytes(command);
            Port.Write(ba, 0, ba.Length);
        }

        public byte[] ReadPort(int chars)
        {
            lock (this)
            {
                try
                {
                    int j;
                    byte[] r = new byte[chars];
                    for (j = 0; j < r.Length; Thread.Sleep(1))
                        j += Port.Read(r, j, chars - j);
                    return r;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public byte[] ReadPort()
        {
            Port.ReadTimeout = 100;
            byte[] r = new byte[1024];
            
            lock (this)
            {
                try
                {
                   
                    int j;

                    for (j = 0; j < r.Length; Thread.Sleep(1))
                        j += Port.Read(r, j, 1024 - j);
                    return r;
                }
                catch (TimeoutException)
                {
                    try
                    {

                        string s = asc.GetString(r);
                        string[] sa = s.Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries);

                        return asc.GetBytes(sa[0]);
                    }
                    catch (Exception) 
                    { 
                        Thread.Sleep(50);
                        DiscardInBuffer();
                        return r;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Port.ReadTimeout = 1000;
                }
            }
        }
    
        //public byte[] ReadPort()
        //{
        //    try
        //    {
        //        int intRecSize = xsp.BytesToRead;
        //        if (intRecSize != 0)
        //        {
        //            byte[] buff = new byte[intRecSize];
        //            xsp.Read(buff, 0, intRecSize);
        //            return buff;
        //        }
        //        else
        //            return null;
        //    }
        //    finally
        //    {
        //        xsp.DiscardInBuffer();
        //    }
        //}

        /// <summary>
        /// If parity checking is enabled and a parity error is received the whole commandline is discarded.
        /// </summary>
        /// <param name="baudrate">baudrate: requested baud rate up to 230400, 9600 is the power up default</param>
        /// <param name="parity">parity: Parity generating and checking 0-default=none, 1=odd, 2=even</param>
        public void ChangeBaudRate(int baudrate, int parity = 0)
        {
            // LAN 연결에서는 baudrate 개념이 없으므로 무시한다.
            if (isLan)
                return;

            try
            {
                if(parity == 0)
                    WritePort(";baud " + baudrate.ToString());
                else
                    WritePort(";baud " + baudrate.ToString() + " " + parity.ToString());

                bRate = baudrate;
                xsp.Close();
                xsp.BaudRate = (int)baudrate;
                xsp.Open();
                DiscardInBuffer();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
