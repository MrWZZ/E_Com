using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface ILoginPanelsEntity : IBaseSceneEntity
    {
        LoginPanelsComponent LoginPanelsComponent { get; }
    }

    public class LoginPanelsComponent : BaseComponent<ILoginPanelsEntity>
    {
        private GameObject loginPanelPre;
        private GameObject loginSettingPanelPre;
        private GameObject selectSavesPanelPre;

        public override void InitComponent()
        {
            Entity.AddEvent(LoginSceneEvent.OpenLoginPanel, OpenPanel);
            Entity.AddEvent(LoginSceneEvent.CloseLoginPanel, ClosePanel);
        }

        public void OpenPanel(object args = null, object argsEx = null)
        {
            LoginPanelsEnum panelsEnum = (LoginPanelsEnum)args;
            switch (panelsEnum)
            {
                case LoginPanelsEnum.LoginPanel:
                    if (loginPanelPre == null)
                    {
                        loginPanelPre = Global.LoadAsset<GameObject>("LoginPanel", "login");
                    }
                    Global.OpenPanel<LoginPanelEntity>(loginPanelPre,UILayerEnum.Center,argsEx);
                    break;
                case LoginPanelsEnum.LoginSettingPanel:
                    if (loginSettingPanelPre == null)
                    {
                        loginSettingPanelPre = Global.LoadAsset<GameObject>("LoginSettingPanel", "login");
                    }
                    Global.OpenPanel<LoginSettingPanelEntity>(loginSettingPanelPre,UILayerEnum.Center,argsEx);
                    break;
                case LoginPanelsEnum.SelectSavesPanel:
                    if (selectSavesPanelPre == null)
                    {
                        selectSavesPanelPre = Global.LoadAsset<GameObject>("SelectSavesPanel", "login");
                    }
                    Global.OpenPanel<SelectSavesPanelEntity>(selectSavesPanelPre,UILayerEnum.Center,argsEx);
                    break;
            }
        }

        public void ClosePanel(object args = null, object argsEx = null)
        {
            LoginPanelsEnum panelsEnum = (LoginPanelsEnum)args;
            switch (panelsEnum)
            {
                case LoginPanelsEnum.LoginPanel:
                    Global.ClosePanel("LoginPanel");
                    break;
                case LoginPanelsEnum.LoginSettingPanel:
                    Global.ClosePanel("LoginSettingPanel");
                    break;
                case LoginPanelsEnum.SelectSavesPanel:
                    Global.ClosePanel("SelectSavesPanel");
                    break;
            }
        }
    }

    public enum LoginPanelsEnum
    {
        LoginPanel,
        LoginSettingPanel,
        SelectSavesPanel,
    }
}
