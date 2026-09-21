using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace LAFLib
{
    [ComVisible(true)]
    [Guid("ECDD4EA9-E361-4979-B329-C5C367206D1F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public partial interface ILafLib
    {
        void Open(int _comport, int _BaudRate = 9600);
        void Close();
        void WritePort(string command);
        byte[] ReadPort(int chars);
        byte[] ReadPort();
        void ChangeBaudRate(int baudrate, int parity = 0);
        void OpenLan(string ip, int tcpPort);
    }
}
