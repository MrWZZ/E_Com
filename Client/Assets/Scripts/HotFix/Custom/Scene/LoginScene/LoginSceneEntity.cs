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
            SetClick("Button", OnClickButton);
            GameObject cude = Global.LoadAsset<GameObject>("Cube", "login");
            Instantiate(cude);
        }

        public void OnClickButton()
        {
            Global.LoadScene<MainSceneEntity>("Main", "main_scene");
        }
    }
}
