using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System;

public class MD5Creator 
{
    public const string MD5_FILE_NAME = "md5.txt";

    public static float curProgress = 0;
    public static float totalProgress = 0;
    public static bool isDone = false;
    public static bool isCancel = false;
    public static string curFileName = "";

    public class MD5Config
    {
        public string name;
        public string md5;

        public MD5Config(string name,string md5)
        {
            this.name = name;
            this.md5 = md5;
        }
    }

    [MenuItem("MD5Creator/CreateSelect")]
    public static void CreateMD5()
    {
        //获取根目录
        if (Selection.activeObject.GetType() != typeof(DefaultAsset))
        {
            EditorUtility.DisplayDialog("提示", "选择的文件不是一个文件夹", "确定");
            return;
        }
        AssetImporter rootPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject));

        string path = rootPath.assetPath;

        ThreadPool.QueueUserWorkItem(o => CreateOperateAsync(path));
    }

    public static void ResetDate()
    {
        curProgress = 0;
        totalProgress = 0;
        isDone = false;
        isCancel = false;
        curFileName = "";
    }

    public static void EditorUpdate()
    {
        if(!isDone)
        {
            if(isCancel)
            {
                EditorUtility.DisplayCancelableProgressBar("进度", "正在取消", 1);
            }
            else
            {
                isCancel = EditorUtility.DisplayCancelableProgressBar(curFileName, $"{curProgress}/{totalProgress}", curProgress / totalProgress);
            }
        }
        else
        {
            EditorUtility.ClearProgressBar();
            EditorApplication.update -= EditorUpdate;
            if(isCancel)
            {
                EditorUtility.DisplayDialog("提示", "生成被取消", "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "生成完成", "确定");
                AssetDatabase.Refresh();
            }
        }
    }

    public static void CreateOperateAsync(string rootPath)
    {
        DirectoryInfo rootDir = new DirectoryInfo(rootPath);
        List<string> allFileList = GetAllFiles(rootDir);
        if (allFileList.Count <= 0)
        {
            Debug.Log("文件夹下无实体文件");
            return;
        }

        ResetDate();
        totalProgress = allFileList.Count;
        EditorApplication.update += EditorUpdate;

        StringBuilder md5Sb = new StringBuilder();
        for (int i = 0; i < allFileList.Count; i++)
        {
            string fileName = allFileList[i].Replace(rootDir.FullName, "");
            string md5 = CreateMd5(allFileList[i]);
            md5Sb.Append($"{fileName},{md5}\n");

            curProgress++;
            curFileName = fileName;
            if (isCancel)
            {
                isDone = true;
                return;
            }
        }

        using (FileStream fs = new FileStream($"{rootPath}/{MD5_FILE_NAME}", FileMode.Create, FileAccess.ReadWrite))
        {
            byte[] jsonBuffer = Encoding.UTF8.GetBytes(md5Sb.ToString());
            fs.Write(jsonBuffer, 0, jsonBuffer.Length);
        }

        isDone = true; 
    }

    public static List<string> GetAllFiles(DirectoryInfo curPath)
    {
        List<string> pathList = new List<string>();
        
        FileInfo[] allFile = curPath.GetFiles();
        foreach (FileInfo fileItem in allFile)
        {
            if (fileItem.Extension == ".meta" || fileItem.Name == MD5_FILE_NAME) { continue; }

            pathList.Add(fileItem.FullName);
        }

        DirectoryInfo[] allDir = curPath.GetDirectories();
        foreach (DirectoryInfo dirItem in allDir)
        {
            List<string> nextList = GetAllFiles(dirItem);
            pathList.AddRange(nextList);
        }

        return pathList;
    }

    public static string CreateMd5(string fullPath)
    {
        string md5 = "";
        using (var fileStream = File.OpenRead(fullPath))
        {
            var fileMD5Bytes = MD5.Create().ComputeHash(fileStream);
            md5 = System.BitConverter.ToString(fileMD5Bytes).Replace("-","");
        }
        return md5;
    }
}
