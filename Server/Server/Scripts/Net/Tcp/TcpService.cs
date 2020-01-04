using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class TcpService
    {
        private const int BUFFER_LENGTH = 1000;
        private const string SERVER_ADDRESS = "127.0.0.1";
        private const int SERVER_PORT = 9999;
        public const int MAX_LISTEN = 100;

        private SocketAsyncEventArgs listenerArgs = new SocketAsyncEventArgs();

        public Socket listenerSocket;

        public Dictionary<EndPoint, TcpChannel> channelDic = new Dictionary<EndPoint, TcpChannel>();

        public TcpService()
        {
            listenerArgs.Completed += ArgsCompleted;

            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); ;
            listenerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listenerSocket.Bind(new IPEndPoint(IPAddress.Parse(SERVER_ADDRESS), SERVER_PORT));
            listenerSocket.Listen(MAX_LISTEN);

            AcceptAsync();
        }

        private void AcceptAsync()
        {
            Console.WriteLine("等待连接");
            listenerArgs.AcceptSocket = null;
            if (!listenerSocket.AcceptAsync(listenerArgs))
            {
                OnAcceptComplete(listenerArgs);
            }
        }

        public void OnAcceptComplete(SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                return;
            }

            Console.WriteLine("连接成功");
            if(!channelDic.ContainsKey(args.AcceptSocket.RemoteEndPoint))
            {
                channelDic.Add(args.AcceptSocket.RemoteEndPoint, new TcpChannel(args.AcceptSocket));
            }
            
            AcceptAsync();
        }

        public void OnDisConnectComplete(SocketAsyncEventArgs args)
        {
            Console.WriteLine("断开链接");
            if(channelDic.ContainsKey(args.AcceptSocket.RemoteEndPoint))
            {
                channelDic.Remove(args.AcceptSocket.RemoteEndPoint);
            }
        }

        private void ArgsCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    OnAcceptComplete(e);
                    break;

                case SocketAsyncOperation.Disconnect:

                    break;
            }
        }
    }
}
