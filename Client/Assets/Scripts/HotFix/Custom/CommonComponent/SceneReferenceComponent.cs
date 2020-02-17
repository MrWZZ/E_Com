using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace HotFix
{
    public interface ISceneReferenceEntity
    {
        TextAsset SceneReferenceConfig { get; }
    }

    public class SceneReferenceComponent : BaseComponent
    {
        public ISceneReferenceEntity Entity { get; private set; }
        private Dictionary<string, GameObject> referenceDic = new Dictionary<string, GameObject>();
        public SceneReferenceComponent Init(ISceneReferenceEntity entity)
        {
            Entity = entity;
            ReadConfig();
            return this;
        }

        private void ReadConfig()
        {
            string config = Entity.SceneReferenceConfig.text;
            string[] lineArr = config.Split('\n');
            foreach (var line in lineArr)
            {
                string[] item = line.Split(',');
                if(referenceDic.ContainsKey(item[0]))
                {
                    Log.Error("场景中存在相同名称的引用:" + item[0]);
                    continue;
                }

                GameObject targetObj;

                int rootIndex = item[1].IndexOf("/");
                if (rootIndex > -1)
                {
                    string rootObjName = item[1].Substring(0, rootIndex);
                    GameObject rootObj = GameObject.Find(rootObjName);
                    string childPath = item[1].Remove(0, rootIndex + 1);
                    targetObj = rootObj.transform.Find(childPath).gameObject;
                }
                else
                {
                    targetObj = GameObject.Find(item[1]);
                }

                referenceDic.Add(item[0], targetObj);
            }
        }

        public T Get<T>(string referenceName) where T : Object
        {
            if(!referenceDic.ContainsKey(referenceName))
            {
                Log.Error($"{referenceName}:场景中不存在该名称的物体");
                return null;
            }

            if(typeof(T) == typeof(GameObject))
            {
                return referenceDic[referenceName] as T;
            }
            else
            {
                T component = referenceDic[referenceName].GetComponent<T>();
                if(component == null)
                {
                    Log.Error($"{referenceName}物体上不存在{typeof(T)}组件");
                    return null;
                }

                return component;
            }
        }
    }
}

