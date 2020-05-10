using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class OriginSceneEntity : BaseSceneEntity,
        ICoroutineEntity
    {
        public CoroutineComponent CoroutineComponent { get; private set; }

        private GameObject sceneLoadingPanelPre;
        private float loadingProgress;
        private bool isInit;
        private float loadSpeed;

        public override void InitScene()
        {
            CoroutineComponent = Add<CoroutineComponent, ICoroutineEntity>(this);
            Global._HotFixEntity.SceneComponent.OnLoadSceneStart += OnLoadSceneStart;
            Global._HotFixEntity.SceneComponent.OnLoadSceneProgress += OnLoadSceneProgress;
            Global._HotFixEntity.SceneComponent.OnLoadSceneEnd += OnLoadSceneEnd;
            LoadGlobalResource();
            loadSpeed = 0.02f;
            Global.LoadScene<LoginSceneEntity>("Login", "login_scene");
        }

        private void LoadGlobalResource()
        {
            // 加载通用资源
            Global.LoadAssetsBundle("origin");
            sceneLoadingPanelPre = Global.LoadAsset<GameObject>("SceneLoadingPanel", "origin");
            Global.UnLoadAssetsBundle("origin");
        }

        private void OnLoadSceneStart()
        {
            if(!isInit) { return; }

            loadingProgress = 0;
            Global.OpenPanel<SceneLoadingPanelEntity>(sceneLoadingPanelPre,UILayerEnum.OnlyTop);
            CoroutineComponent.EStartCoroutine("DoingLoadSceneProgress", DoingLoadSceneProgress(), CorMutilEnum.Cover);
        }

        private void OnLoadSceneProgress(float progress)
        {
            if (!isInit) { return; }
            loadingProgress = progress;
        }

        private void OnLoadSceneEnd()
        {
            if (!isInit) {
                isInit = true;
                return; 
            }
            loadingProgress = 1;
        }

        public IEnumerator DoingLoadSceneProgress()
        {
            float tempLoadPro = 0;
            while(tempLoadPro < 1)
            {
                if(tempLoadPro < loadingProgress)
                {
                    tempLoadPro += loadSpeed;
                    TriggerEvent(OriginSceneEvent.SetProgressText, tempLoadPro);
                }
                yield return null;
            }
            Global.ClosePanel("SceneLoadingPanel", UILayerEnum.OnlyTop);
        }
    }
}
