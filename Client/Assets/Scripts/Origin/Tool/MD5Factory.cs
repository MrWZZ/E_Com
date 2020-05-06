using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public static class MD5Factory
    {
        public static String BuildFileMd5(String filename)
        {
            String filemd5 = null;
            try
            {
                using (var fileStream = File.OpenRead(filename))
                {
                    var md5 = MD5.Create();
                    var fileMD5Bytes = md5.ComputeHash(fileStream);                                  
                    filemd5 = System.BitConverter.ToString(fileMD5Bytes);
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex);
            }
            return filemd5;
        }
    }
}
