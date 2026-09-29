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
        int readTimeoutMs = 1000;
        const int PollSliceMs = 20;

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
            }
            catch
            {
                client.Close();
                throw;
            }
        }

        public bool IsOpen { get { return client.Connected; } }

        // 주의: NetworkStream.ReadTimeout(= SO_RCVTIMEO)은 여기서 절대 사용하지 않는다.
        // Winsock 사양상 블로킹 소켓에서 수신 타임아웃이 한 번이라도 발동하면 그 연결은
        // 내부적으로 손상되어, 이후 같은 소켓으로의 Send가 전부
        // "현재 연결은 사용자의 호스트 시스템의 소프트웨어의 의해 중단되었습니다"(WSAECONNABORTED)로 실패한다.
        // 따라서 타임아웃은 Socket.Poll로 데이터 도착 여부만 확인해서 직접 구현한다.
        public int ReadTimeout
        {
            get { return readTimeoutMs; }
            set { readTimeoutMs = value; }
        }

        public void Write(byte[] buffer, int offset, int count) { stream.Write(buffer, offset, count); }

        public int Read(byte[] buffer, int offset, int count)
        {
            int waited = 0;
            while (!client.Client.Poll(PollSliceMs * 1000, SelectMode.SelectRead))
            {
                waited += PollSliceMs;
                if (waited >= readTimeoutMs)
                    throw new TimeoutException("Read timeout");
            }

            // Poll이 true를 반환한 경우 데이터가 있거나 연결이 끊긴 것이다(둘 다 Read 호출로 확인 가능).
            int n = stream.Read(buffer, offset, count);
            if (n == 0)
                throw new IOException("Connection closed by remote host.");
            return n;
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
