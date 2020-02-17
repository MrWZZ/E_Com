using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SceneReferenceHierarchyEditor 
{
    static SceneReferenceHierarchyEditor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
    }

    private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) { return; }
        if (obj.GetComponent<SceneReferenceUnit>() != null)
        {
            GUIStyle style = new GUIStyle()
            {
                padding = {left = EditorStyles.label.padding.left,top = EditorStyles.label.padding.top + 1},
                normal = { textColor = Color.blue},
                fontSize = 10,
                fontStyle = FontStyle.Bold
            };

            GUI.Label(selectionRect, "SR", style);
        }
    }
}
