using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class LoginSettingPanelEntity : BasePanelEntity
    {
        public override void InitPanel(object args = null)
        {
            SetClick("returnButton", OnClickReturnButton);
        }

        public void OnClickReturnButton()
        {
            TriggerEvent(LoginSceneEvent.OpenLoginPanel, LoginPanelsEnum.LoginPanel);
            TriggerEvent(LoginSceneEvent.CloseLoginPanel, LoginPanelsEnum.LoginSettingPanel);
        }
    }
}
