using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace HotFix
{
    public interface IResourcesReferenceEntity
    {

    }

    public class ResourcesReferenceComponent : BaseComponent
    {
        public IResourcesReferenceEntity Entity { get; private set; }
        private Dictionary<string, Dictionary<string, string>> bundle_NamePath = new Dictionary<string, Dictionary<string, string>>();
        public ResourcesReferenceComponent Init(IResourcesReferenceEntity entity)
        {
            Entity = entity;

            return this;
        }

        public T LoadResources<T>(string fileName,string bundleName) where T : Object
        {
            T loadObj = null;
#if UNITY_EDITOR

            int index = bundleName.IndexOf("_");
            if(index < 0)
            {
                Log.Error("bundle名称不正确");
                return loadObj;
            }
            else
            {
                string gameName = bundleName.Substring(0, index);
                string configPath = $"{Application.dataPath}/Game/{gameName}/Assetbundle/{bundleName}/Configs/{bundleName.Replace("_Bundle","")}ResourceReference.txt";
                if(!File.Exists(configPath))
                {
                    Log.Error("资源配置文件不存在");
                    return loadObj;
                }

                Dictionary<string, string> referencePathDic;
                if (!bundle_NamePath.ContainsKey(bundleName))
                {
                    referencePathDic = new Dictionary<string, string>();
                    bundle_NamePath.Add(bundleName, referencePathDic);
                    string[] contentLines = File.ReadAllLines(configPath);
                    foreach (var line in contentLines)
                    {
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        string[] lineArr = line.Split(',');

                        if (referencePathDic.ContainsKey(lineArr[0]))
                        {
                            Log.Error($"{lineArr[0]},{lineArr[1]}\n资源中存在相同名字的引用,请修改其中的一个。");
                            continue;
                        }

                        referencePathDic.Add(lineArr[0], lineArr[1]);
                    }
                }
                else
                {
                    referencePathDic = bundle_NamePath[bundleName];
                }

                if(!referencePathDic.ContainsKey(fileName))
                {
                    Log.Error($"{bundleName}中不存在{fileName}物体");
                    return loadObj;
                }

                loadObj = AssetDatabase.LoadAssetAtPath(referencePathDic[fileName], typeof(T)) as T;
            }
#else

#endif
            if (loadObj == null)
            {
                Log.Error($"{name}:加载失败");
            }
            return loadObj;
        }
    }
}

