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

        public override void InitComponent()
        {
            
        }

        public void Load<T>(string sceneName,string sceneBundleName,Action<float> onProgress = null) where T : BaseSceneEntity
        {
            Entity.AssetBundleComponent.LoadAssetsBundle(sceneBundleName);
            StartCoroutine(LoadSceneAsync<T>(sceneName, sceneBundleName, onProgress));
        }

        private IEnumerator LoadSceneAsync<T>(string sceneName, string sceneBundleName, Action<float> onProgress = null) where T : BaseSceneEntity
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = true;

            while(!asyncOperation.isDone)
            {
                onProgress?.Invoke(asyncOperation.progress);
                yield return null;
            }

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
}
