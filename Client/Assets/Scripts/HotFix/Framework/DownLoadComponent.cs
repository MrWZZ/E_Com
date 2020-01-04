using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace HotFix
{
    public struct AssetBundleStruct
    {
        public string abName;
        public string abPath;
        public string md5;
        public long size;
    }

    public class DownLoadComponent : MonoBehaviour
    {
        public Dictionary<string, AssetBundleStruct> AB_Dic = new Dictionary<string, AssetBundleStruct>();

        public void LoadBundle(string path,Action<AssetBundle> completeAction,Action<long, long> progressAction = null,Action failAction = null)
        {
            string url = "ftp://192.168.10.89" + path;
            string filePath = Application.streamingAssetsPath + path;
            //判断本地是否存在资源
            if (File.Exists(path))
            {
                //资源版本验证 todo

                //断点续传
            }
            else
            {
                StartCoroutine(LoadAssetBundle(url, filePath,completeAction,progressAction,failAction));
            }
        }

        public IEnumerator LoadAssetBundle(string url,string filePath, Action<AssetBundle> completeAction, Action<long, long> progressAction, Action failAction)
        {
            //Head方法可以获取到文件的全部长度
            UnityWebRequest huwr = UnityWebRequest.Head(url);
            yield return huwr.SendWebRequest();
            if (huwr.isNetworkError || huwr.isHttpError)
            {
                Debug.Log(huwr.error);
                failAction?.Invoke();
            }
            else
            {
                //首先拿到文件的全部长度
                long totalLength = long.Parse(huwr.GetResponseHeader("Content-Length"));
                string dirPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    //当前文件长度
                    long nowFileLength = fs.Length;
                    if (nowFileLength < totalLength)
                    {
                        //设置文件写入位置
                        fs.Seek(nowFileLength, SeekOrigin.Begin);
                        UnityWebRequest uwr = UnityWebRequest.Get(url);

                        //设置从指定位置开始下载
                        uwr.SetRequestHeader("Range", "bytes=" + nowFileLength + "-" + totalLength);
                        uwr.SendWebRequest();

                        //从该索引处继续下载
                        long index = 0;     
                        while (!uwr.isDone)
                        {
                            yield return null;
                            byte[] data = uwr.downloadHandler.data;
                            if (data != null)
                            {
                                long length = data.Length - index;
                                //写入文件
                                fs.Write(data, (int)index, (int)length); 
                                index += length;
                                nowFileLength += length;
                                progressAction?.Invoke(nowFileLength, totalLength);
                                //如果下载完成了
                                if (nowFileLength >= totalLength) 
                                {
                                    completeAction?.Invoke(DownloadHandlerAssetBundle.GetContent(uwr));
                                }
                            }
                        }

                        if (uwr.isNetworkError || uwr.isHttpError)
                        {
                            failAction?.Invoke();
                            Debug.Log(uwr.error);
                        }
                    }
                }
            }
        }

        //下载文件，现在应该只能下载小文件，过大的文件应该需要分段下载
        public static IEnumerator DownloadFile(string url, string filePath)
        {
            //Head方法可以获取到文件的全部长度
            UnityWebRequest huwr = UnityWebRequest.Head(url); 
            yield return huwr.SendWebRequest();
            if (huwr.isNetworkError || huwr.isHttpError) 
            {
                Debug.Log(huwr.error); 
            }
            else
            {
                //首先拿到文件的全部长度
                long totalLength = long.Parse(huwr.GetResponseHeader("Content-Length")); 
                string dirPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dirPath)) 
                {
                    Directory.CreateDirectory(dirPath);
                }

                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    //当前文件长度
                    long nowFileLength = fs.Length; 
                    if (nowFileLength < totalLength)
                    {
                        //设置文件写入位置
                        fs.Seek(nowFileLength, SeekOrigin.Begin);       
                        UnityWebRequest uwr = UnityWebRequest.Get(url);
                        
                        //设置从指定位置开始下载
                        uwr.SetRequestHeader("Range", "bytes=" + nowFileLength + "-" + totalLength);
                        uwr.SendWebRequest();
                        
                        long index = 0;     //从该索引处继续下载
                        while (!uwr.isDone) 
                        {
                            yield return null;
                            byte[] data = uwr.downloadHandler.data;
                            if (data != null)
                            {
                                long length = data.Length - index;
                                fs.Write(data, (int)index, (int)length); //写入文件
                                index += length;
                                nowFileLength += length;
                                if (nowFileLength >= totalLength) //如果下载完成了
                                {
                                    Debug.Log("download complete");
                                }
                            }
                        }

                        if (uwr.isNetworkError || uwr.isHttpError) 
                        {
                            Debug.Log(uwr.error); 
                        }
    
                    }
                }
            }
        }

    }
}
