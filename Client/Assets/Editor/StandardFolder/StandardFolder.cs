using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class StandardFolder 
{
    [MenuItem("Assets/CreateGameFolder", priority = 999)]
    public static void CreateGameFolder()
    {
        Object folder = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(folder);

        //资源
        CreateSceneBundleFolderOperate($"{path}/Assetbundle/{folder.name}_Bundle");
        
        //场景
        if (!Directory.Exists($"{path}/Scene"))
        {
            Directory.CreateDirectory($"{path}/Scene");
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/CreateSceneBundleFolder",priority = 999)]
    public static void CreateSceneBundleFolder()
    {
        Object folder = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(folder);

        CreateSceneBundleFolderOperate(path);
    }

    private static void CreateSceneBundleFolderOperate(string path)
    {
        //配置文件
        if (!Directory.Exists($"{path}/Configs"))
        {
            Directory.CreateDirectory($"{path}/Configs");
        }
        //材质
        if (!Directory.Exists($"{path}/Materials"))
        {
            Directory.CreateDirectory($"{path}/Materials");
        }
        //预制体
        if (!Directory.Exists($"{path}/Prefabs"))
        {
            Directory.CreateDirectory($"{path}/Prefabs");
        }
        //图片
        if (!Directory.Exists($"{path}/Sprites"))
        {
            Directory.CreateDirectory($"{path}/Sprites");
        }

        AssetDatabase.Refresh();
    }
}
