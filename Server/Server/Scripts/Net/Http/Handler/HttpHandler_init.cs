using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace Server
{
    public class HttpHandler_init
    {
        private static List<Dictionary<string, Func<HttpListenerContext, string>>> handlerList = new List<Dictionary<string, Func<HttpListenerContext, string>>>();

        public static void RegisterHandler()
        {
            HttpHandlerManager.AddHander("/init", Deal_init);
            HttpHandlerManager.AddHander("/init/client_info", Deal_init_clientinfo);
        }

        /// <summary>
        /// /init
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Deal_init(HttpListenerContext context)
        {
            Log.Success("DEAL:/init");

            string json = ConfigReader.ReadConfig("/Http/init.json");

            return json;
        }

        public class Struct_init
        {
            public string client_version;
            public string client_url;
            public string dll_version;
            public string dll_url;
        }


        /// <summary>
        /// /init/client_info
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Deal_init_clientinfo(HttpListenerContext context)
        {
            Log.Success("DEAL:/init/client_info");
            string result = "";


            return result;
        }
    }
}
