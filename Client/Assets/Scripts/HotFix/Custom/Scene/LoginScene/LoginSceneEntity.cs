using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace HotFix
{
    public class LoginSceneEntity : BaseSceneEntity
    {
        public override void InitScene()
        {
            var loginPanel = Global.LoadAsset<GameObject>("LoginPanel", "login");
            Global.OpenPanel<LoginPanelEntity>(loginPanel);
        }

    }
}
