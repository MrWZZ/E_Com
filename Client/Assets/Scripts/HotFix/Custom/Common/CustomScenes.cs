using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public partial class Custom
    {
        public static void LoadScene(CustomSceneEnum sceneEnum)
        {
            Global.CloseAllPanel();
            switch(sceneEnum)
            {
                case CustomSceneEnum.Login:
                    Global.LoadScene<LoginSceneEntity>("Login", "login_scene");
                    break;
                case CustomSceneEnum.Main:
                    Global.LoadScene<MainSceneEntity>("Main", "main_scene");
                    break;
            }
        }
    }

    public enum CustomSceneEnum
    {
        Login,
        Main
    }
}
