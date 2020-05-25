using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface IScenesLoadingProgressEntity : IBaseSceneEntity
    {
        CoroutineComponent CoroutineComponent { get; }
        ScenesLoadingProgressComponent ScenesLoadingProgressComponent { get; }
    }

    public class ScenesLoadingProgressComponent : BaseComponent<IScenesLoadingProgressEntity>
    {
        private float loadingProgress;
        private bool isInit;
        private float loadSpeed;

        public override void InitComponent()
        {
            loadSpeed = 0.02f;
            // 监听事件
            Global._HotFixEntity.SceneComponent.OnLoadSceneStart += OnLoadSceneStart;
            Global._HotFixEntity.SceneComponent.OnLoadSceneProgress += OnLoadSceneProgress;
            Global._HotFixEntity.SceneComponent.OnLoadSceneEnd += OnLoadSceneEnd;
        }

        private void OnLoadSceneStart()
        {
            if (!isInit) { return; }

            loadingProgress = 0;
            Entity.TriggerEvent(OriginSceneEvent.OpenOriginPanel, OriginPanelsEnum.SceneLoadingPanel);
            Entity.CoroutineComponent.EStartCoroutine("DoingLoadSceneProgress", DoingLoadSceneProgress(), CorMutilEnum.Cover);
        }

        private void OnLoadSceneProgress(float progress)
        {
            if (!isInit) { return; }
            loadingProgress = progress;
        }

        private void OnLoadSceneEnd()
        {
            if (!isInit)
            {
                isInit = true;
                return;
            }
            loadingProgress = 1;
        }

        public IEnumerator DoingLoadSceneProgress()
        {
            float tempLoadPro = 0;
            while (tempLoadPro < 1)
            {
                if (tempLoadPro < loadingProgress)
                {
                    tempLoadPro += loadSpeed;
                    Entity.TriggerEvent(OriginSceneEvent.SetProgressText, tempLoadPro);
                }
                yield return null;
            }
            Entity.TriggerEvent(OriginSceneEvent.CloseOriginPanel, OriginPanelsEnum.SceneLoadingPanel);
        }
    }
}
