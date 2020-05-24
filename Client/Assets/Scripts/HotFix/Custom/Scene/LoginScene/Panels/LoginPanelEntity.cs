using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    public class LoginPanelEntity : BasePanelEntity
    {
        public override void InitPanel(object args = null)
        {
            SetClick("startButton", OnClickStartButton);
            SetClick("settingButton", OnClickSettingButton);
            SetClick("exitButton", OnClickExitButton);
        }

        public void OnClickStartButton()
        {
            TriggerEvent(LoginSceneEvent.OpenLoginPanel, LoginPanelsEnum.SelectSavesPanel);
            TriggerEvent(LoginSceneEvent.CloseLoginPanel, LoginPanelsEnum.LoginPanel);
        }

        public void OnClickSettingButton()
        {
            TriggerEvent(LoginSceneEvent.OpenLoginPanel, LoginPanelsEnum.LoginSettingPanel);
            TriggerEvent(LoginSceneEvent.CloseLoginPanel, LoginPanelsEnum.LoginPanel);
        }

        public void OnClickExitButton()
        {
            Custom.ExitGame();
        }
    }
}
