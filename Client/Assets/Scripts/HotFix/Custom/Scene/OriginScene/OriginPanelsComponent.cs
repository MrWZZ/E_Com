using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface IOriginPanelsEntity : IBaseSceneEntity
    {
        OriginPanelsComponent OriginPanelsComponent { get; }
    }

    public class OriginPanelsComponent : BaseComponent<IOriginPanelsEntity>
    {
        private GameObject sceneLoadingPanelPre;

        public override void InitComponent()
        {
            Entity.AddEvent(OriginSceneEvent.OpenOriginPanel, OpenPanel);
            Entity.AddEvent(OriginSceneEvent.CloseOriginPanel, ClosePanel);
        }

        public void OpenPanel(object args = null,object argsEx = null)
        {
            OriginPanelsEnum panelsEnum = (OriginPanelsEnum)args;
            switch (panelsEnum)
            {
                case OriginPanelsEnum.SceneLoadingPanel:
                    if (sceneLoadingPanelPre == null)
                    {
                        sceneLoadingPanelPre = Global.LoadAsset<GameObject>("SceneLoadingPanel", "origin");
                    }
                    Global.OpenPanel<SceneLoadingPanelEntity>(sceneLoadingPanelPre, UILayerEnum.OnlyTop, argsEx);
                    break;
            }
        }

        public void ClosePanel(object args = null, object argsEx = null)
        {
            OriginPanelsEnum panelsEnum = (OriginPanelsEnum)args;
            switch (panelsEnum)
            {
                case OriginPanelsEnum.SceneLoadingPanel:
                    Global.ClosePanel("SceneLoadingPanel", UILayerEnum.OnlyTop);
                    break;
            }
        }
    }

    public enum OriginPanelsEnum
    {
        SceneLoadingPanel,
    }
}
