using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace Server
{
    public class HttpServer
    {
        private static HttpListener httpObj;

        public static void RunHttpServer()
        {
            //提供一个简单的、可通过编程方式控制的 HTTP 协议侦听器。此类不能被继承。
            httpObj = new HttpListener();
            
            Struct_httpServer data = ConfigReader.ReadConfig<Struct_httpServer>("/Http/http_server.json");
            httpObj.Prefixes.Add(data.listen_url);

            //启动监听器
            httpObj.Start();
            //异步监听客户端请求，当客户端的网络请求到来时会自动执行Result委托
            //该委托没有返回值，有一个IAsyncResult接口的参数，可通过该参数获取context对象
            httpObj.BeginGetContext(Result, null);
            Log.Info($"Web服务器已启动，正在等待客户端请求,时间:{DateTime.Now.ToString()}");
        }

        public class Struct_httpServer
        {
            public string listen_url;
        }

        private static void Result(IAsyncResult ar)
        {

            //继续异步监听
            httpObj.BeginGetContext(Result, null);
            //获得context对象
            var context = httpObj.EndGetContext(ar);
            var request = context.Request;
            var response = context.Response;

            Log.Info($"接到新的请求，时间:{DateTime.Now.ToString()}\r\n地址:{request.RawUrl}");
            ////如果是js的ajax请求，还可以设置跨域的ip地址与参数
            //context.Response.AppendHeader("Access-Control-Allow-Origin", "*");//后台跨域请求，通常设置为配置文件
            //context.Response.AppendHeader("Access-Control-Allow-Headers", "ID,PW");//后台跨域参数设置，通常设置为配置文件
            //context.Response.AppendHeader("Access-Control-Allow-Method", "post");//后台跨域请求设置，通常设置为配置文件

            context.Response.AddHeader("Content-type", "text/html");//添加响应头信息
            context.Response.ContentType = "text/html;charset=utf-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8

            context.Response.ContentEncoding = Encoding.UTF8;
            string returnObj = null;//定义返回客户端的信息

            //此处添加对不同地址请求的处理
            string handlerUrl = request.RawUrl.IndexOf("?") > -1 ? request.RawUrl.Split("?")[0] : request.RawUrl;
            returnObj = HttpHandlerManager.DealHandler(handlerUrl, context);

            var returnByteArr = Encoding.UTF8.GetBytes(returnObj);//设置客户端返回信息的编码
            try
            {
                using (var stream = response.OutputStream)
                {
                    //把处理信息返回到客户端
                    stream.Write(returnByteArr, 0, returnByteArr.Length);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"网络蹦了：{ex.ToString()}");
            }
        }

        //private string HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        //{
        //    string data = null;
        //    try
        //    {
        //        var byteList = new List<byte>();
        //        var byteArr = new byte[2048];
        //        int readLen = 0;
        //        int len = 0;
        //        //接收客户端传过来的数据并转成字符串类型
        //        do
        //        {
        //            readLen = request.InputStream.Read(byteArr, 0, byteArr.Length);
        //            len += readLen;
        //            byteList.AddRange(byteArr);
        //        } while (readLen != 0);
        //        data = Encoding.UTF8.GetString(byteList.ToArray(), 0, len);

        //        //获取得到数据data可以进行其他操作
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusDescription = "404";
        //        response.StatusCode = 404;
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine($"在接收数据时发生错误:{ex.ToString()}");
        //        return $"在接收数据时发生错误:{ex.ToString()}";//把服务端错误信息直接返回可能会导致信息不安全，此处仅供参考
        //    }
        //    response.StatusDescription = "200";//获取或设置返回给客户端的 HTTP 状态代码的文本说明。
        //    response.StatusCode = 200;// 获取或设置返回给客户端的 HTTP 状态代码。
        //    Console.ForegroundColor = ConsoleColor.Green;
        //    Console.WriteLine($"接收数据完成:{data.Trim()},时间：{DateTime.Now.ToString()}");
        //    return $"接收数据完成";
        //}
    }
}
