using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class TcpChannel
    {
        private Socket acceptSocket;
        private SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();

        public TcpChannel(Socket socket)
        {
            Console.WriteLine("new channel:" + socket.RemoteEndPoint.ToString());
            acceptSocket = socket;
            receiveArgs.Completed += ArgsCompleted;
            sendArgs.Completed += ArgsCompleted;

            ReceiveAsync();
        }

        public void ReceiveAsync()
        {
            receiveArgs.SetBuffer(new byte[100], 0, 100);
            if (!acceptSocket.ReceiveAsync(receiveArgs))
            {
                OnReceiveComplete(receiveArgs);
            }
        }

        private void OnReceiveComplete(SocketAsyncEventArgs e)
        {
            ReceiveAsync();

            if (e.SocketError != SocketError.Success || e.BytesTransferred == 0)
            {
                Console.WriteLine("接收失败");
                return;
            }

            string msg = Encoding.Unicode.GetString(e.Buffer, 0, e.BytesTransferred);
            Console.WriteLine("接收成功:" + msg);
        }

        private void OnSendComplete(SocketAsyncEventArgs e)
        {

        }

        private void ArgsCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    OnReceiveComplete(e);
                    break;
                case SocketAsyncOperation.Send:
                    OnSendComplete(e);
                    break;
            }
        }
    }
}
