using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Server
{
    public class HttpHandlerManager
    {
        private static Dictionary<string, Func<HttpListenerContext,string>> handlerDic = new Dictionary<string, Func<HttpListenerContext, string>>();

        static HttpHandlerManager()
        {
            RegisterAllHandler();
        }

        public static void RegisterAllHandler()
        {
            HttpHandler_init.RegisterHandler();
            HttpHandler_game.RegisterHandler();
        }

        public static string DealHandler(string url, HttpListenerContext context)
        {
            try
            {
                if (handlerDic.ContainsKey(url))
                {
                    context.Response.StatusDescription = "200";
                    context.Response.StatusCode = 200;
                    return handlerDic[url].Invoke(context);
                }
                else
                {
                    context.Response.StatusDescription = "404";
                    context.Response.StatusCode = 404;
                    Log.Warning($"请求未处理：{url}");
                    return "访问地址错误";
                }
            }
            catch(Exception e)
            {
                string guid = Guid.NewGuid().ToString();
                context.Response.StatusDescription = "404";
                context.Response.StatusCode = 404;
                Log.Error($"ID:{guid} \r\n 时间：{DateTime.Now} \r\n 错误：{e.ToString()} \r\n");
                return $"ID:{guid} \r\n 时间：{DateTime.Now}，服务器产生异常";
            }
        }

        public static void AddHander(string url, Func<HttpListenerContext, string> handler)
        {
            if(handlerDic.ContainsKey(url))
            {
                Log.Warning($"{url}：处理已存在，无法重复添加。");
            }
            else
            {
                handlerDic.Add(url, handler);
            }
        }
    }
}
