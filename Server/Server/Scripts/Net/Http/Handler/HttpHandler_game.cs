using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Server
{
    public class HttpHandler_game
    {
        private static List<Dictionary<string, Func<HttpListenerContext, string>>> handlerList = new List<Dictionary<string, Func<HttpListenerContext, string>>>();

        public static void RegisterHandler()
        {
            HttpHandlerManager.AddHander("/game", Deal_game);
            HttpHandlerManager.AddHander("/game/game_info", Deal_game_gameinfo);
        }

        /// <summary>
        /// /init
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Deal_game(HttpListenerContext context)
        {
            Log.Success("DEAL:/game");
            string result = "";


            return result;
        }

        /// <summary>
        /// /game/game_info
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Deal_game_gameinfo(HttpListenerContext context)
        {
            Log.Success("DEAL:/game/game_info");
            string result = "";


            return result;
        }
    }
}
