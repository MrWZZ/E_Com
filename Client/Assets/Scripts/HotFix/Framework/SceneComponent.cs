using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotFix
{
    public interface ISceneEntity
    {
        AssetBundleComponent AssetBundleComponent { get; }
        SceneComponent SceneComponent { get; }
    }

    public class SceneComponent : BaseComponent<ISceneEntity>
    {
        public BaseSceneEntity CurScene { get; private set; }
        private Dictionary<string, SceneData> sceneDataDic;
        public Action OnLoadSceneStart;
        public Action<float> OnLoadSceneProgress;
        public Action OnLoadSceneEnd;

        public override void InitComponent()
        {
            sceneDataDic = new Dictionary<string, SceneData>();
        }

        public void RegisterScene<SceneType>(string sceneName,string sceneBundleName) where SceneType : BaseSceneEntity
        {
            if(sceneDataDic.ContainsKey(sceneName))
            {
                Log.Warning($"{sceneName}场景已被注册，无法重复注册。");
                return;
            }

            SceneData data = new SceneData();
            data.sceneName = sceneName;
            data.sceneBundleName = sceneBundleName;
            data.sceneEnityType = typeof(SceneType);
            sceneDataDic.Add(sceneName, data);
        }

        public void Load<T>(string sceneName,string sceneBundleName) where T : BaseSceneEntity
        {
            OnLoadSceneStart?.Invoke();
            Entity.AssetBundleComponent.LoadAssetsBundle(sceneBundleName);
            StartCoroutine(LoadSceneAsync<T>(sceneName, sceneBundleName));
        }

        private IEnumerator LoadSceneAsync<T>(string sceneName, string sceneBundleName) where T : BaseSceneEntity
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            
            asyncOperation.allowSceneActivation = true;
            while (!asyncOperation.isDone)
            {
                OnLoadSceneProgress?.Invoke(asyncOperation.progress);
                yield return null;
            }
            OnLoadSceneEnd?.Invoke();

            // 默认向场景的第一个根物体挂载场景启动脚本
            Scene curScene = SceneManager.GetActiveScene();
            GameObject[] gameObjects = curScene.GetRootGameObjects();
            if (gameObjects.Length > 0)
            {
                CurScene?._UnLoadDependBundle();
                CurScene = gameObjects[0].AddComponent<T>();
                CurScene.InitEntity();
                CurScene._LoadDependBundle();
                CurScene.InitScene();
            }
            else
            {
                Log.Error($"场景中无根物体：{sceneName}");
            }

            if (Global.GlobalConfig.isUseAssetBundle)
            {
                Entity.AssetBundleComponent.UnLoadAssetBundle(sceneBundleName);
            }
        }
    }

    public class SceneData
    {
        public string sceneName;
        public string sceneBundleName;
        public Type sceneEnityType;
    }
}
