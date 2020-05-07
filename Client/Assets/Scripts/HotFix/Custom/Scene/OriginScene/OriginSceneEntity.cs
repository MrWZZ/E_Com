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
        private SceneLoadingPanelEntity sceneLoadingPanelEntity;
        private float loadingProgress;
        private bool isInit;

        public override void InitScene()
        {
            CoroutineComponent = Add<CoroutineComponent, ICoroutineEntity>(this);
            Global._HotFixEntity.SceneComponent.OnLoadSceneStart += OnLoadSceneStart;
            Global._HotFixEntity.SceneComponent.OnLoadSceneProgress += OnLoadSceneProgress;
            Global._HotFixEntity.SceneComponent.OnLoadSceneEnd += OnLoadSceneEnd;
            LoadGlobalResource();

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
            sceneLoadingPanelEntity = Global.OpenPanel<SceneLoadingPanelEntity>(sceneLoadingPanelPre,UILayerEnum.OnlyTop);
        }

        private void OnLoadSceneProgress(float progress)
        {
            if (!isInit) { return; }

            sceneLoadingPanelEntity.SetProgressText(progress);
            loadingProgress = progress;
        }

        private void OnLoadSceneEnd()
        {
            if (!isInit) {
                isInit = true;
                return; 
            }
            
            CoroutineComponent.EStartCoroutine("DoingLoadSceneProgress", DoingLoadSceneProgress(),CorMutilEnum.Cover);
        }

        public IEnumerator DoingLoadSceneProgress()
        {
            while(loadingProgress <= 1)
            {
                loadingProgress += 0.01f;
                sceneLoadingPanelEntity.SetProgressText(loadingProgress);
                yield return null;
            }
            Global.ClosePanel("SceneLoadingPanel", UILayerEnum.OnlyTop);
        }
    }
}
