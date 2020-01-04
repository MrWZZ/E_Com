using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Server
{
    public class ConfigReader
    {
        private const string ROOT_PATH = "Resources\\Config";

        public static string ReadConfig(string path)
        {
            path = path.Replace("/", "\\");
            string configPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + ROOT_PATH + path;
            
            if(!File.Exists(configPath))
            {
                Log.Warning($"文件不存在：{configPath}");
                return "";
            }

            using(FileStream fs = new FileStream(configPath,FileMode.Open,FileAccess.Read))
            {
                StreamReader reader = new StreamReader(fs);
                return reader.ReadToEnd();
            }
        }

        public static T ReadConfig<T>(string path)
        {
            string json = ReadConfig(path);
            if(string.IsNullOrEmpty(json))
            {
                return default(T);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                Log.Warning($"配置解析失败:{path}");
                return default(T);
            }
            
        }
    }
}
