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
            Button button = SetClick("Button", OnClickButton);
        }

        public void OnClickButton()
        {
            Global.LoadScene<MainSceneEntity>("Main", "main_scene");
            Global.ClosePanel("LoginPanel");
        }
    }
}
