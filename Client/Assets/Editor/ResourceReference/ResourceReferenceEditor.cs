using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using System.Threading;
using Object = UnityEngine.Object;
using System;

[CustomEditor(typeof(ResourceReference))]
public class ResourceReferenceEditor : Editor
{

    private static string assetParentPath;
    private static Dictionary<string, Object> referenceObjDic = new Dictionary<string, Object>();
    private static bool isCancel = false;
    private static OperateStatusEnum operateStatus = OperateStatusEnum.None;

    private enum OperateStatusEnum
    {
        None,
        FindFiles,
        Complete,
        Cancel,
        Error,
    }

    private class QueskData
    {
        public ManualResetEvent mrEvent;
        public string bundlePath;
        public string bundleName;
    }

    private static string ResourceReferenceConfigPath
    {
        get
        {
            Scene curScene = SceneManager.GetActiveScene();
            string path = curScene.path.Replace($"Scene/{curScene.name}.unity", $"Assetbundle/{curScene.name}_Bundle/Configs/{curScene.name}ResourceReference.txt");
            return path;
        }
    }

    public void OnEnable()
    {
        referenceObjDic.Clear();
        ReadReferenceConfig();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        foreach (var pair in referenceObjDic)
        {
            EditorGUILayout.ObjectField(pair.Key, pair.Value, typeof(GameObject), false);
        }
    }

    [MenuItem("ResourceReference/CreateAllFolderConfig", priority = 0)]
    public static void CreateAllFolderConfig()
    {
        //遍历大的场景分类
        string gameFolderPath = Application.dataPath + "/Game";
        DirectoryInfo gameFolderDir = new DirectoryInfo(gameFolderPath);
        DirectoryInfo[] subGameFolderArr = gameFolderDir.GetDirectories();
        List<DirectoryInfo> allBundleFolderList = new List<DirectoryInfo>();
        foreach (var directory in subGameFolderArr)
        {
            string curGameFolderPath = directory.FullName.Replace("\\", "/") + "/Assetbundle";
            DirectoryInfo curBundleFolderDic = new DirectoryInfo(curGameFolderPath);
            DirectoryInfo[] subBundleFolderArr = curBundleFolderDic.GetDirectories();
            allBundleFolderList.AddRange(subBundleFolderArr);
        }

        StartQuesk(allBundleFolderList);
    }

    [MenuItem("ResourceReference/CreateSelectFolderConfig", priority = 0)]
    public static void CreateSelectFolderConfig()
    {
        Object folder = Selection.activeObject;
        if(!folder.name.EndsWith("_Bundle"))
        {
            Debug.LogError("选择的文件夹不是标准文件夹（xxx_Bundle），检查是否选错了文件夹。");
            return;
        }

        StartQuesk(new List<DirectoryInfo>() { new DirectoryInfo(AssetDatabase.GetAssetPath(folder)) });
    }

    private static void StartQuesk(List<DirectoryInfo> bundleFolderList)
    {
        isCancel = false;
        operateStatus = OperateStatusEnum.None;
        EditorApplication.update += EditorUpdate;
        assetParentPath = Application.dataPath.Replace("/Assets", "");
        ThreadPool.QueueUserWorkItem(ThreadQuesk, bundleFolderList);
    }

    private static void EditorUpdate()
    {
        switch (operateStatus)
        {
            case OperateStatusEnum.None:
                break;
            case OperateStatusEnum.Complete:
                EditorApplication.update -= EditorUpdate;
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("提示", "生成完成", "确定");
                AssetDatabase.Refresh();
                break;
            case OperateStatusEnum.Cancel:
                EditorApplication.update -= EditorUpdate;
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("提示", "任务已取消", "确定");
                break;
            case OperateStatusEnum.Error:
                EditorApplication.update -= EditorUpdate;
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("提示", "任务存在错误，请查看日志。", "确定");
                break;
            default:
                if (isCancel)
                {
                    EditorUtility.DisplayCancelableProgressBar("提示", "正在取消", 1);
                }
                else
                {
                    isCancel = EditorUtility.DisplayCancelableProgressBar($"提示", "任务进行中", 0);
                }
                break;
        }
    }

    private static void ReadReferenceConfig()
    {
        if (!File.Exists(ResourceReferenceConfigPath))
        {
            return;
        }

        string[] contentLines = File.ReadAllLines(ResourceReferenceConfigPath);
        foreach (var line in contentLines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            string[] lineArr = line.Split(',');

            Object targetObj = AssetDatabase.LoadAssetAtPath(lineArr[1], typeof(Object)); 

            if (referenceObjDic.ContainsKey(lineArr[0]))
            {
                Debug.LogError($"{lineArr[0]},{lineArr[1]}\n资源中存在相同名字的引用,请修改其中的一个。");
                continue;
            }

            referenceObjDic.Add(lineArr[0], targetObj);
        }
    }

    #region 异步任务

    private static void ThreadQuesk(object obj)
    {
        List<DirectoryInfo> bundleFolderList = (List<DirectoryInfo>)obj;
        List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();

        try
        {
            foreach (var bundleFolder in bundleFolderList)
            {
                ManualResetEvent mre = new ManualResetEvent(false);
                manualEvents.Add(mre);
                QueskData data = new QueskData()
                {
                    mrEvent = mre,
                    bundlePath = bundleFolder.FullName.Replace("\\", "/"),
                    bundleName = bundleFolder.Name.Replace("_Bundle", "")
                };
                ThreadPool.QueueUserWorkItem(CreateOperateAsync, data);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        WaitHandle.WaitAll(manualEvents.ToArray());
        operateStatus = OperateStatusEnum.Complete;
    }

    public static void CreateOperateAsync(object obj)
    {
        QueskData data = (QueskData)obj;

        DirectoryInfo rootDir = new DirectoryInfo(data.bundlePath);
        Dictionary<string, string> allFileDic = GetAllFiles(rootDir);

        //在寻找文件途中可取消或者发生异常，可以提前退出
        if (operateStatus == OperateStatusEnum.Error || operateStatus == OperateStatusEnum.Cancel)
        {
            return;
        }

        StringBuilder configBuilder = new StringBuilder();
        foreach (var file in allFileDic)
        {
            if (isCancel)
            {
                operateStatus = OperateStatusEnum.Cancel;
                return;
            }

            string assetFilePath = file.Value.Replace("\\","/").Replace(assetParentPath+"/", "");
            configBuilder.AppendLine(file.Key + "," + assetFilePath);
        }

        string configPath = $"{data.bundlePath}/Configs/{data.bundleName}ResourceReference.txt";

        using (FileStream fs = new FileStream(configPath, FileMode.Create))
        {
            byte[] buffer = Encoding.UTF8.GetBytes(configBuilder.ToString());
            fs.Write(buffer, 0, buffer.Length);
        }

        data.mrEvent.Set();
    }

    public static Dictionary<string, string> GetAllFiles(DirectoryInfo curPath)
    {
        if (isCancel)
        {
            operateStatus = OperateStatusEnum.Cancel;
            return new Dictionary<string, string>();
        }

        operateStatus = OperateStatusEnum.FindFiles;

        Dictionary<string, string> filePathDic = new Dictionary<string, string>();

        FileInfo[] allFile = curPath.GetFiles();
        foreach (FileInfo fileItem in allFile)
        {
            if (fileItem.Extension == ".meta") { continue; }
            string curFileName = fileItem.Name.Replace(fileItem.Extension,"");
            filePathDic.Add(curFileName, fileItem.FullName);
        }

        DirectoryInfo[] allDir = curPath.GetDirectories();
        foreach (DirectoryInfo dirItem in allDir)
        {
            Dictionary<string, string> nextDic = GetAllFiles(dirItem);

            foreach (var file in nextDic)
            {
                if(filePathDic.ContainsKey(file.Key))
                {
                    operateStatus = OperateStatusEnum.Error;
                    Debug.LogError($"{file.Key}，{file.Value}\n同一个bundle中存在相同的文件名，不允许存在相同文件名，请修改其中一个的文件名后重新生成。");
                    return new Dictionary<string, string>();
                }
                else
                {
                    filePathDic.Add(file.Key, file.Value);
                }
            }
        }

        return filePathDic;
    }

    #endregion
}
