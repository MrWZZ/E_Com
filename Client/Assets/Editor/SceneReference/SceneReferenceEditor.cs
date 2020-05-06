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
    private static Dictionary<string, GameObject> referenceObjDic = new Dictionary<string, GameObject>();

    public void OnEnable()
    {
        UpdateReference();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        foreach (var pair in referenceObjDic)
        {
            EditorGUILayout.ObjectField(pair.Key, pair.Value, typeof(GameObject), true);
        }
    }

    public static void UpdateReference()
    {
        referenceObjDic.Clear();
        SceneReferenceUnit[] gameObjects = FindObjectsOfType<SceneReferenceUnit>();
        if(gameObjects.Length > 0)
        {
            foreach (var unit in gameObjects)
            {
                if(referenceObjDic.ContainsKey(unit.name))
                {
                    Debug.Log($"场景中存在相同名字的SR引用：{unit.name}");
                    continue;
                }
                else
                {
                    referenceObjDic.Add(unit.name, unit.gameObject);
                }
            }
        }
    }

    [MenuItem("GameObject/SceneReference/Add", priority = 0)]
    public static void AddSceneReference()
    {
        GameObject selectObj = Selection.activeGameObject;
        if (selectObj == null)
        {
            return;
        }

        //检查是否存在相同key
        if (referenceObjDic.ContainsKey(selectObj.name))
        {
            Selection.objects = new Object[] { referenceObjDic[selectObj.name].gameObject, selectObj.gameObject };
            Debug.Log($"场景中存在相同名字的引用，请修改名称后重新添加: {selectObj.name}");
            return;
        }

        if (selectObj.GetComponent<SceneReferenceUnit>() == null)
        {
            selectObj.AddComponent<SceneReferenceUnit>();
        }

        UpdateReference();
    }

    [MenuItem("GameObject/SceneReference/Remove", priority = 0)]
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

        UpdateReference();
    }
}
