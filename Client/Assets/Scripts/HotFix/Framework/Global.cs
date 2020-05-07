using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public static class Global
    {
        public static HotFixEntity _HotFixEntity;

        private static GlobalConfig globalConfig;
        public static GlobalConfig GlobalConfig
        {
            get
            {
                if(globalConfig == null)
                {
                    TextAsset json = Resources.Load<TextAsset>("GlobalConfig");
                    globalConfig = JsonUtility.FromJson<GlobalConfig>(json.text);
                }
                return globalConfig;
            }
        }

        public static void LoadScene<T>(string sceneName, string sceneBundleName, Action<float> onProgress = null) 
            where T : BaseSceneEntity
        {
            _HotFixEntity.SceneComponent.Load<T>(sceneName, sceneBundleName, onProgress);
        }

        public static T LoadAsset<T>(string fileName, string bundleName) where T : UnityEngine.Object
        {
            return _HotFixEntity.AssetBundleComponent.LoadAsset<T>(fileName, bundleName);
        }

        public static void OpenPanel<T>(GameObject panelPre, UILayerEnum uiLayer = UILayerEnum.Center, object args = null) where T : BasePanelEntity
        {
            _HotFixEntity.PanelComponent.OpenPanel<T>(panelPre, uiLayer, args);
        }

        public static void ClosePanel(string panelName, UILayerEnum uiLayer = UILayerEnum.Center)
        {
            _HotFixEntity.PanelComponent.ClosePanel(panelName, uiLayer);
        }

        public static void LoadAssetsBundle(string bundleName)
        {
            _HotFixEntity.AssetBundleComponent.LoadAssetsBundle(bundleName);
        }

        public static void UnLoadAssetsBundle(string bundleName)
        {
            _HotFixEntity.AssetBundleComponent.UnLoadAssetBundle(bundleName);
            _HotFixEntity.AssetBundleComponent.UnLoadAsset(bundleName);
        }

    }

    public class GlobalConfig
    {
        public bool isOnLine;
        public bool isUseAssetBundle;
        public string serverInitUrl;
        public string clientVersion;
        public string scriptVersion;
    }
}
