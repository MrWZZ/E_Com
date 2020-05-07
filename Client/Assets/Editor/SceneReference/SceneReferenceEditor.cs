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
    private static Dictionary<string, List<GameObject>> referenceObjDic = new Dictionary<string, List<GameObject>>();
    private static SceneReference sceneReference;

    public void OnEnable()
    {
        sceneReference = target as SceneReference;
        UpdateReference();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        foreach (var pair in referenceObjDic)
        {
            var curList = pair.Value;
            foreach (var go in curList)
            {
                EditorGUILayout.ObjectField(go.name, go, typeof(GameObject), true);
            }
        }
    }

    public static void UpdateReference()
    {
        referenceObjDic.Clear();
        if(sceneReference == null) { return; }
        SceneReferenceUnit[] gameObjects = sceneReference.gameObject.GetComponentsInChildren<SceneReferenceUnit>(true);
        if(gameObjects.Length > 0)
        {
            foreach (var unit in gameObjects)
            {
                List<GameObject> curList;
                referenceObjDic.TryGetValue(unit.name,out curList);
                if(curList == null)
                {
                    curList = new List<GameObject>();
                    referenceObjDic.Add(unit.name, curList);
                }
                curList.Add(unit.gameObject);
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
