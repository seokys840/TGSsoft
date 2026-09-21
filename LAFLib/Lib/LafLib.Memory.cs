using System;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace LAFLib
{
    [ComVisible(true)]
    [Guid("9EF554CE-95AD-47B2-8022-4EF0FA9DCC9C")]
    [ClassInterface(ClassInterfaceType.None)]
    public partial class LAF : ILafLib
    {

        public Thread TransceiverTaskInst;
        
        SerialPort xsp = new SerialPort("COM1");
        public SerialPort Xsp
        {
            get { return xsp; }
        }

        ITransport port;

        bool isLan = false;
        public bool IsLan
        {
            get { return isLan; }
        }

        string ipAddress = "";
        public string IPAddress
        {
            get { return ipAddress; }
        }

        int tcpPort;
        public int TcpPort
        {
            get { return tcpPort; }
        }

        int comport;
        public int COMport
        {
            get { return comport; }
        }

        int bRate = 9600;
        public int BaudRate
        {
            get { return bRate; }
        }

        
        bool isExit = false;
    }
}
