using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public static class Log
    {
        public static void Info(string message)
        {
            Debug.Log(message);
        }

        public static void Success(string message)
        {
            Debug.Log($"<color='#00ff00'>{message}</color>");
        }

        public static void Warning(string message)
        {
            Debug.Log($"<color='#ffff00'>{message}</color>");
        }

        public static void Error(string message)
        {
            Debug.LogError(message);
        }

        public static void Error(Exception e)
        {
            Debug.LogError(e.ToString());
        }
        
        // 所有框架操作提示
        public static void Test(string message)
        {
            if(!Global.GlobalConfig.isShowTestDebug) { return; }
            Debug.Log(message);
        }
    }
}
