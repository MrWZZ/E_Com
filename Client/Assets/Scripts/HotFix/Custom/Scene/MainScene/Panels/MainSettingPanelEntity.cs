using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public class MainSettingPanelEntity : BasePanelEntity
    {
        public override void InitPanel(object args = null)
        {
            SetClick("closeButton", OnClickCloseButton);
            SetClick("exitButton",OnClickExitButton);
            SetClick("returnLoginButton", OnClickReturnLoginButton);
        }

        public void OnClickCloseButton()
        {
            TriggerEvent(MainSceneEvent.CloseMainPanel, MainPanelsEnum.MainSettingPanel);
        }

        public void OnClickReturnLoginButton()
        {
            Custom.LoadScene(CustomSceneEnum.Login);
        }

        public void OnClickExitButton()
        {
            Custom.ExitGame();
        }
    }
}
