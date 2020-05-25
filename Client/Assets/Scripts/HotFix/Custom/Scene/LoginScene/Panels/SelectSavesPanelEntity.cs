using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class SelectSavesPanelEntity : BasePanelEntity
    {
        public override void InitPanel(object args = null)
        {
            SetClick("returnButton", OnClickReturnButton);
            SetClick("saveSlot0", OnClickSaveSlot);
            SetClick("saveSlot1", OnClickSaveSlot);
            SetClick("saveSlot2", OnClickSaveSlot);
        }

        public void OnClickReturnButton()
        {
            TriggerEvent(LoginSceneEvent.OpenLoginPanel, LoginPanelsEnum.LoginPanel);
            TriggerEvent(LoginSceneEvent.CloseLoginPanel, LoginPanelsEnum.SelectSavesPanel);
        }

        public void OnClickSaveSlot()
        {
            Custom.LoadScene(CustomSceneEnum.Main);
        }
    }
}
