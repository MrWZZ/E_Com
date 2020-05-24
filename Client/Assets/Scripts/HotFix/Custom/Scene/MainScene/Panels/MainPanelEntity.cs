using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    public class MainPanelEntity : BasePanelEntity
    {
        public override void InitPanel(object args = null)
        {
            SetClick("settingButton", OnClickSettingButton);
        }

        public void OnClickSettingButton()
        {
            TriggerEvent(MainSceneEvent.OpenMainPanel, MainPanelsEnum.MainSettingPanel);
        }
    }
}
