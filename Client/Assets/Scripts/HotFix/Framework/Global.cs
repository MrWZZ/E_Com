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

        private static GlobalDataObject globalConfig;
        public static GlobalDataObject GlobalConfig
        {
            get
            {
                if(globalConfig == null)
                {
                    globalConfig = Resources.Load<GlobalDataObject>("GlobalDataObject");
                }
                return globalConfig;
            }
        }

        #region Global

        public static T AddEntity<T>(GameObject attachObj) where T : BaseEntity
        {
            T entity = attachObj.AddComponent<T>();
            if(entity == null)
            {
                Log.Error($"依附失败：{attachObj.name},{typeof(T)}");
            }

            return entity;
        }

        #endregion

        #region Scene

        public static void LoadScene<T>(string sceneName, string sceneBundleName) 
            where T : BaseSceneEntity
        {
            _HotFixEntity.SceneComponent.Load<T>(sceneName, sceneBundleName);
        }

        public static void OpenPanel<T>(GameObject panelPre, UILayerEnum uiLayer = UILayerEnum.Center, object args = null) 
            where T : BasePanelEntity
        {
            _HotFixEntity.PanelComponent.OpenPanel<T>(panelPre, uiLayer, args);
        }

        public static void ClosePanel(string panelName, UILayerEnum uiLayer = UILayerEnum.Center)
        {
            _HotFixEntity.PanelComponent.ClosePanel(panelName, uiLayer);
        }

        #endregion

        #region AssetBundle
        public static void LoadAssetsBundle(string bundleName)
        {
            _HotFixEntity.AssetBundleComponent.LoadAssetsBundle(bundleName);
        }

        public static void UnLoadAssetsBundle(string bundleName)
        {
            _HotFixEntity.AssetBundleComponent.UnLoadAssetBundle(bundleName);
            _HotFixEntity.AssetBundleComponent.UnLoadAsset(bundleName);
        }

        public static T LoadAsset<T>(string fileName, string bundleName) where T : UnityEngine.Object
        {
            return _HotFixEntity.AssetBundleComponent.LoadAsset<T>(fileName, bundleName);
        }
        #endregion

        #region Event
        public static void TriggerEvent(string eventName, object args = null)
        {
            _HotFixEntity.EventComponent.TriggerEvent(eventName, args);
        }

        public static void AddEvent(string eventName, BaseSceneEntity entity, EventComponentHandler eventHandler)
        {
            
            _HotFixEntity.EventComponent.AddEvent(eventName, entity, eventHandler);
        }

        public static void RemoveEvent(string eventName, BaseSceneEntity entity)
        {
            _HotFixEntity.EventComponent.RemoveEvent(eventName, entity);
            
        }

        public static void RemoveAllEntityEvent(BaseSceneEntity entity)
        {
            _HotFixEntity.EventComponent.RemoveAllEntityEvent(entity);
        }

        #endregion
    }

}
