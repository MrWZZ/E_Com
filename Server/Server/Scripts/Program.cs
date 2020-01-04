using System;
using System.Threading;
using System.Net;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Info($"服务器程序启动");

            HttpServer.RunHttpServer();

            string pragma = "";

            while(pragma != "quit")
            {
                pragma = Console.ReadLine();
            }
        }
    }
}
