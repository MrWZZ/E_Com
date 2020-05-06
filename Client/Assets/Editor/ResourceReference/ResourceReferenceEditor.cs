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
    public static string ResourceReferenceConfigPath;
    private static ResourceReference ResourceReference;
    private static string assetParentPath;
    private static Dictionary<string, Dictionary<string, Object>> referenceObjDic = new Dictionary<string, Dictionary<string, Object>>();
    private static bool isCancel = false;
    private static OperateStatusEnum operateStatus = OperateStatusEnum.None;
    private static string changeKey = "";

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

    public void OnEnable()
    {
        changeKey = "";
    }

    public override void OnInspectorGUI()
    {
        ResourceReference = target as ResourceReference;
        DrawDependBundleList();
        ReadConfig();
        foreach (var pair in referenceObjDic)
        {
            var bundleName = pair.Key;
            var bundleList = pair.Value;
            GUILayout.Label($"{bundleName}");
            foreach (var item in bundleList)
            {
                EditorGUILayout.ObjectField(item.Key, item.Value, typeof(GameObject), false);
            }
            EditorGUILayout.Space();
        }
    }

    [MenuItem("Assets/ResourceReference/All", priority = 999)]
    public static void CreateAllFolderConfig()
    {
        ResourceReferenceConfigPath = $"{Application.dataPath}/Temp/Configs";
        //遍历大的场景分类
        string gameFolderPath = Application.dataPath + "/Game";
        DirectoryInfo gameFolderDir = new DirectoryInfo(gameFolderPath);
        DirectoryInfo[] subGameFolderArr = gameFolderDir.GetDirectories();
        List<DirectoryInfo> allBundleFolderList = new List<DirectoryInfo>();
        foreach (var directory in subGameFolderArr)
        {
            string curGameFolderPath = directory.FullName.Replace("\\", "/") + "/AssetsBundle";
            if(!Directory.Exists(curGameFolderPath))
            {
                Debug.LogError($"该路径不存在：{curGameFolderPath}");
                continue;
            }
            DirectoryInfo curBundleFolderDic = new DirectoryInfo(curGameFolderPath);
            DirectoryInfo[] subBundleFolderArr = curBundleFolderDic.GetDirectories();
            allBundleFolderList.AddRange(subBundleFolderArr);
        }

        StartQuesk(allBundleFolderList);
    }


    [MenuItem("Assets/ResourceReference/Select", priority = 999)]
    public static void CreateSelectFolderConfig()
    {
        ResourceReferenceConfigPath = $"{Application.dataPath}/Temp/Configs";
        Object folder = Selection.activeObject;
        if (!folder.name.EndsWith("_Bundle"))
        {
            Debug.LogError("选择的文件夹不是标准文件夹（xxx_Bundle），检查是否选错了文件夹。");
            return;
        }

        StartQuesk(new List<DirectoryInfo>() { new DirectoryInfo(AssetDatabase.GetAssetPath(folder)) });
    }

    private static void ReadConfig()
    {
        ResourceReferenceConfigPath = $"{Application.dataPath}/Temp/Configs";
        referenceObjDic.Clear();
        if(ResourceReference == null) { return; }
        List<string> dependBundleList = ResourceReference.dependBundleList;

        foreach (var bundleName in dependBundleList)
        {
            var rrDic = new Dictionary<string, Object>();
            string configPath = $"{ResourceReferenceConfigPath}/{bundleName.ToLower()}ResourceReference.txt";
            if (!File.Exists(configPath))
            {
                continue;
            }

            string[] contentLines = File.ReadAllLines(configPath);
            foreach (var line in contentLines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                string[] lineArr = line.Split(',');
                Object targetObj = AssetDatabase.LoadAssetAtPath(lineArr[1], typeof(Object));
                if (rrDic.ContainsKey(lineArr[0]))
                {
                    Debug.LogError($"{lineArr[0]},{lineArr[1]}\n资源中存在相同名字的引用,请修改其中的一个。");
                    continue;
                }

                rrDic.Add(lineArr[0], targetObj);
            }

            referenceObjDic.Add(bundleName, rrDic);
        }
    }

    
    private static void DrawDependBundleList()
    {
        if(ResourceReference == null) { return; }
        string[] tempArr = ResourceReference.dependBundleList.ToArray();
        foreach (var bundleName in tempArr)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(bundleName);
            if (GUILayout.Button("删除"))
            {
                ResourceReference.dependBundleList.Remove(bundleName);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        changeKey = EditorGUILayout.TextField(changeKey);
        if (GUILayout.Button("添加"))
        {
            if(!string.IsNullOrEmpty(changeKey) && !ResourceReference.dependBundleList.Contains(changeKey))
            {
                ResourceReference.dependBundleList.Add(changeKey);
                changeKey = "";
            }
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
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
                    EditorApplication.update -= EditorUpdate;
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("提示", "任务已取消", "确定");
                }
                else
                {
                    isCancel = EditorUtility.DisplayCancelableProgressBar($"提示", "任务进行中", 0);
                }
                break;
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
                if (!bundleFolder.Name.EndsWith("_Bundle"))
                {
                    Debug.LogError($"文件夹不是标准文件夹（xxx_Bundle）:。{bundleFolder.FullName}");
                    continue;
                }

                string bundlePath = bundleFolder.FullName.Replace("\\", "/");
                string bundleName = bundleFolder.Name.Replace("_Bundle", "");

                ManualResetEvent mre = new ManualResetEvent(false);
                manualEvents.Add(mre);
                QueskData data = new QueskData()
                {
                    mrEvent = mre,
                    bundlePath = bundlePath,
                    bundleName = bundleName
                };
                ThreadPool.QueueUserWorkItem(CreateOperateAsync, data);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        if(manualEvents.Count > 0)
        {
            WaitHandle.WaitAll(manualEvents.ToArray());
        }
        operateStatus = OperateStatusEnum.Complete;
    }

    public static void CreateOperateAsync(object obj)
    {
        QueskData data = (QueskData)obj;

        DirectoryInfo rootDir = new DirectoryInfo(data.bundlePath);
        var allFileDic = GetAllAssetBundleFiles(rootDir);
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

            string assetFilePath = file.Value.Replace("\\", "/").Replace(assetParentPath + "/", "");
            configBuilder.AppendLine(file.Key + "," + assetFilePath);
        }

        string rawPath = ResourceReferenceConfigPath;
        if (!Directory.Exists(rawPath))
        {
            Directory.CreateDirectory(rawPath);
        }

        string configPath = $"{rawPath}/{data.bundleName.ToLower()}ResourceReference.txt";

        using (FileStream fs = new FileStream(configPath, FileMode.Create))
        {
            byte[] buffer = Encoding.UTF8.GetBytes(configBuilder.ToString());
            fs.Write(buffer, 0, buffer.Length);
        }

        data.mrEvent.Set();

    }

    public static Dictionary<string, string> GetAllAssetBundleFiles(DirectoryInfo curPath)
    {
        if (isCancel)
        {
            operateStatus = OperateStatusEnum.Cancel;
            return default;
        }

        operateStatus = OperateStatusEnum.FindFiles;

        var filePathDic = new Dictionary<string, string>();

        FileInfo[] allFile = curPath.GetFiles();
        foreach (FileInfo fileItem in allFile)
        {
            if (fileItem.Extension == ".meta") { continue; }

            try
            {
                string curFileName = fileItem.Name.Replace(fileItem.Extension, "");
                if (filePathDic.ContainsKey(curFileName))
                {
                    Debug.LogError($"有重复名称引用：{curFileName}\n原资源：{filePathDic[curFileName]}\n重复资源：{fileItem.FullName}");

                    operateStatus = OperateStatusEnum.Error;
                    return default;
                }
                filePathDic.Add(curFileName, fileItem.FullName);
            }
            catch(Exception e)
            {
                Debug.LogError(e.ToString());
                operateStatus = OperateStatusEnum.Error;
                return default;
            }
        }

        DirectoryInfo[] allDir = curPath.GetDirectories();
        foreach (DirectoryInfo dirItem in allDir)
        {
            var nextDic = GetAllAssetBundleFiles(dirItem);
            if (isCancel)
            {
                operateStatus = OperateStatusEnum.Cancel;
                return default;
            }
            if(nextDic == null) { continue; }
            foreach (var file in nextDic)
            {
                if (filePathDic.ContainsKey(file.Key))
                {
                    Debug.LogError($"有重复名称引用：{file.Key}\n原资源：{filePathDic[file.Key]}\n重复资源：{file.Value}");

                    operateStatus = OperateStatusEnum.Error;
                    return default;
                }
                filePathDic.Add(file.Key, file.Value);
            }
        }

        return filePathDic;
    }

    #endregion
}
