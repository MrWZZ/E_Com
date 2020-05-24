using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface IMainPanelsEntity : IBaseSceneEntity
    {
        MainPanelsComponent MainPanelsComponent { get; }
    }

    public class MainPanelsComponent : BaseComponent<IMainPanelsEntity>
    {
        private GameObject mainPanelPre;
        private GameObject mainSettingPanelPre;

        public override void InitComponent()
        {
            Entity.AddEvent(MainSceneEvent.OpenMainPanel, OpenPanel);
            Entity.AddEvent(MainSceneEvent.CloseMainPanel, ClosePanel);
        }

        public void OpenPanel(object args = null, object argsEx = null)
        {
            MainPanelsEnum panelsEnum = (MainPanelsEnum)args;
            switch (panelsEnum)
            {
                case MainPanelsEnum.MainPanel:
                    if (mainPanelPre == null)
                    {
                        mainPanelPre = Global.LoadAsset<GameObject>("MainPanel", "main");
                    }
                    Global.OpenPanel<MainPanelEntity>(mainPanelPre,UILayerEnum.Center,argsEx);
                    break;
                case MainPanelsEnum.MainSettingPanel:
                    if (mainSettingPanelPre == null)
                    {
                        mainSettingPanelPre = Global.LoadAsset<GameObject>("MainSettingPanel", "main");
                    }
                    Global.OpenPanel<MainSettingPanelEntity>(mainSettingPanelPre,UILayerEnum.Center,argsEx);
                    break;
            }
        }

        public void ClosePanel(object args = null, object argsEx = null)
        {
            MainPanelsEnum panelsEnum = (MainPanelsEnum)args;
            switch (panelsEnum)
            {
                case MainPanelsEnum.MainPanel:
                    Global.ClosePanel("MainPanel");
                    break;
                case MainPanelsEnum.MainSettingPanel:
                    Global.ClosePanel("MainSettingPanel");
                    break;
            }
        }
    }

    public enum MainPanelsEnum
    {
        MainPanel,
        MainSettingPanel,
    }
}
