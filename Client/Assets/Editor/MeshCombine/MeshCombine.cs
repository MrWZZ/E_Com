using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace Assets.Editor.MeshCombine
{
    public class MeshCombine
    {
        [MenuItem("MeshCombine/combineObj")]
        public static void CombineObj()
        {
            UnityEngine.Object select = Selection.activeObject;

            if(select.GetType() != typeof(GameObject))
            {
                EditorUtility.DisplayDialog("提示","选中物体非游戏对象","确定");
                return;
            }
            GameObject go = new GameObject();

            MeshFilter[] meshFilters = ((GameObject)select).GetComponentsInChildren<MeshFilter>();  //获取 所有子物体的网格
            CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];           //新建一个合并组，长度与 meshfilters一致
            for (int i = 0; i < meshFilters.Length; i++)                                  
            {
                combineInstances[i].mesh = meshFilters[i].sharedMesh;                               
                combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;        //本地坐标转矩阵，赋值
            }
            Mesh newMesh = new Mesh();                                  //声明一个新网格对象
            newMesh.CombineMeshes(combineInstances);                    //将combineInstances数组传入函数
            go.AddComponent<MeshFilter>().sharedMesh = newMesh;
            Material defaultMat = new Material(Shader.Find("Standard"));
            go.AddComponent<MeshRenderer>().material = defaultMat;

            go.name = select.name + "(combine)";
            Scene curScene = SceneManager.GetActiveScene();
            SceneManager.MoveGameObjectToScene(go, curScene);

            string path = $"Assets/MeshComine/{select.name}";
            if (Directory.Exists(path))
            {
                Directory.Delete(path,true);
            }
            Directory.CreateDirectory(path);
            
            AssetDatabase.CreateAsset(newMesh, $"{path}/{select.name}.asset");
            AssetDatabase.CreateAsset(defaultMat, $"{path}/{select.name}.mat");

            Selection.activeObject = newMesh;
            AssetDatabase.Refresh();
        }
    }
}
