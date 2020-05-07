using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class StandardFolder 
{
    [MenuItem("Assets/StandardFolder/GameFolder", priority = 999)]
    public static void CreateGameFolder()
    {
        Object folder = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(folder);

        CreateRawResourcesFolder(path);
        CreateSceneBundleFolderOperate($"{path}/AssetsBundle/{folder.name}_Bundle");

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/StandardFolder/BundleFolder", priority = 999)]
    public static void CreateSceneBundleFolder()
    {
        Object folder = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(folder);

        CreateSceneBundleFolderOperate(path);
    }

    private static void CreateRawResourcesFolder(string path)
    {
        CreateDirectory($"{path}/Scene");
        CreateDirectory($"{path}/RawAssets");
    }

    private static void CreateSceneBundleFolderOperate(string path)
    {
        //配置文件
        CreateDirectory($"{path}/Configs");
        //材质
        CreateDirectory($"{path}/Materials");
        //预制体
        CreateDirectory($"{path}/Prefabs");
        //图片
        CreateDirectory($"{path}/Sprites");
        //面板
        CreateDirectory($"{path}/Panels");

        AssetDatabase.Refresh();
    }

    private static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
