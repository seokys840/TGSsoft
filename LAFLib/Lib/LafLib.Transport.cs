using System;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;

namespace LAFLib
{
    /// <summary>
    /// 통신 방식(Serial / LAN)을 추상화한 내부 인터페이스.
    /// Read 는 타임아웃 시 TimeoutException 을 던진다.
    /// </summary>
    internal interface ITransport
    {
        bool IsOpen { get; }
        int ReadTimeout { get; set; }
        void Write(byte[] buffer, int offset, int count);
        int Read(byte[] buffer, int offset, int count);
        void DiscardInBuffer();
        void Close();
    }

    internal class SerialTransport : ITransport
    {
        readonly SerialPort sp;

        public SerialTransport(SerialPort port)
        {
            sp = port;
        }

        public bool IsOpen { get { return sp.IsOpen; } }

        public int ReadTimeout
        {
            get { return sp.ReadTimeout; }
            set { sp.ReadTimeout = value; }
        }

        public void Write(byte[] buffer, int offset, int count) { sp.Write(buffer, offset, count); }
        public int Read(byte[] buffer, int offset, int count) { return sp.Read(buffer, offset, count); }
        public void DiscardInBuffer() { sp.DiscardInBuffer(); }
        public void Close() { sp.Close(); }
    }

    internal class TcpTransport : ITransport
    {
        readonly TcpClient client;
        readonly NetworkStream stream;

        public TcpTransport(string host, int port, int connectTimeoutMs)
        {
            client = new TcpClient();
            try
            {
                IAsyncResult ar = client.BeginConnect(host, port, null, null);
                if (!ar.AsyncWaitHandle.WaitOne(connectTimeoutMs, false))
                    throw new TimeoutException("Connect timeout: " + host + ":" + port);
                client.EndConnect(ar);

                client.NoDelay = true;
                stream = client.GetStream();
                stream.ReadTimeout = 1000;
            }
            catch
            {
                client.Close();
                throw;
            }
        }

        public bool IsOpen { get { return client.Connected; } }

        public int ReadTimeout
        {
            get { return stream.ReadTimeout; }
            set { stream.ReadTimeout = value; }
        }

        public void Write(byte[] buffer, int offset, int count) { stream.Write(buffer, offset, count); }

        public int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                int n = stream.Read(buffer, offset, count);
                if (n == 0)
                    throw new IOException("Connection closed by remote host.");
                return n;
            }
            catch (IOException ex)
            {
                SocketException se = ex.InnerException as SocketException;
                if (se != null && se.SocketErrorCode == SocketError.TimedOut)
                    throw new TimeoutException("Read timeout", ex);
                throw;
            }
        }

        public void DiscardInBuffer()
        {
            byte[] tmp = new byte[256];
            while (stream.DataAvailable)
                stream.Read(tmp, 0, tmp.Length);
        }

        public void Close()
        {
            client.Close();
        }
    }
}
