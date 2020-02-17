using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(SceneReference))]
public class SceneReferenceEditor : Editor
{
    private static string SceneReferenceConfigPath
    {
        get
        {
            Scene curScene = SceneManager.GetActiveScene();
            string path = curScene.path.Replace($"Scene/{curScene.name}.unity", $"Assetbundle/{curScene.name}_Bundle/Configs/{curScene.name}SceneReference.txt");
            return path;
        }
    }

    private static Dictionary<string, GameObject> referenceObjDic = new Dictionary<string, GameObject>();

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
            EditorGUILayout.ObjectField(pair.Key, pair.Value, typeof(GameObject), true);
        }
    }

    [MenuItem("SceneReference/CreateConfig", priority = 0)]
    public static void CreateConfig()
    {
        Dictionary<string, Transform> checkSameKeyDic = new Dictionary<string, Transform>();

        Object[] SceneReferenceObjArr = FindObjectsOfType(typeof(SceneReferenceUnit));
        StringBuilder transPath = new StringBuilder();

        foreach (var sceneReferenceObj in SceneReferenceObjArr)
        {
            transPath.Insert(0, "\n");
            //遍历层级结构，获取路径
            Transform curTrans = ((SceneReferenceUnit)sceneReferenceObj).transform;
            
            //检查是否存在相同key
            if (checkSameKeyDic.ContainsKey(curTrans.name))
            {
                Selection.objects = new Object[] { checkSameKeyDic[curTrans.name].gameObject, curTrans.gameObject };
                Debug.LogError($"{curTrans.name}：场景中存在相同名字的引用，请修改名称后重新生成。");
                return;
            } 

            checkSameKeyDic.Add(curTrans.name, curTrans);

            while (curTrans != null)
            {
                transPath.Insert(0, curTrans.name);
                if (curTrans.parent != null)
                {
                    transPath.Insert(0, "/");
                }
                curTrans = curTrans.parent;
            }
            transPath.Insert(0, sceneReferenceObj.name + ",");
        }
        //删除末尾的回车
        if(transPath.Length > 0)
        {
            transPath.Remove(transPath.Length - 1, 1);
        }

        using (FileStream fs = new FileStream(SceneReferenceConfigPath, FileMode.Create))
        {
            byte[] buff = Encoding.UTF8.GetBytes(transPath.ToString());
            fs.Write(buff, 0, buff.Length);
        }

        AssetDatabase.Refresh();
    }

    private static void ReadReferenceConfig()
    {
        if (!File.Exists(SceneReferenceConfigPath))
        {
            return;
        }

        string[] contentLines = File.ReadAllLines(SceneReferenceConfigPath);
        foreach (var line in contentLines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            string[] lineArr = line.Split(',');

            GameObject targetObj;

            int rootIndex = lineArr[1].IndexOf("/");
            if (rootIndex > -1)
            {
                string rootObjName = lineArr[1].Substring(0, rootIndex);
                GameObject rootObj = GameObject.Find(rootObjName);
                string childPath = lineArr[1].Remove(0, rootIndex + 1);
                targetObj = rootObj.transform.Find(childPath).gameObject;
            }
            else
            {
                targetObj = GameObject.Find(lineArr[1]);
            }

            if (targetObj.GetComponent<SceneReferenceUnit>() == null)
            {
                targetObj.AddComponent<SceneReferenceUnit>();
            }

            if (referenceObjDic.ContainsKey(lineArr[0]))
            {
                Debug.LogError($"场景中存在相同名字的引用:{lineArr[0]},{lineArr[1]}");
                continue;
            }

            referenceObjDic.Add(lineArr[0], targetObj);
        }
    }

    [MenuItem("GameObject/AddSceneReference", priority = 0)]
    public static void AddSceneReference()
    {
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            return;
        }

        //如果里面没有引用，先读取，确保下面的判断存在依据
        if(referenceObjDic.Count == 0)
        {
            referenceObjDic.Clear();
            ReadReferenceConfig();
        }

        //检查是否存在相同key
        if (referenceObjDic.ContainsKey(selectObj.name))
        {
            Selection.objects = new Object[] { referenceObjDic[selectObj.name].gameObject, selectObj.gameObject };
            Debug.LogError($"{selectObj.name}：场景中存在相同名字的引用，请修改名称后重新添加。");
            return;
        }

        if (selectObj.GetComponent<SceneReferenceUnit>() == null)
        {
            selectObj.AddComponent<SceneReferenceUnit>();
        }

        CreateConfig();
    }

    [MenuItem("GameObject/RemoveSceneReference", priority = 0)]
    public static void RemoveSceneReference()
    {
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            return;
        }

        SceneReferenceUnit unit = selectObj.GetComponent<SceneReferenceUnit>();
        if (unit != null)
        {
            DestroyImmediate(unit);
        }

        CreateConfig();
    }
}
