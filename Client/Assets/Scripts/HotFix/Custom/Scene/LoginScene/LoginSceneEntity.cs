using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace HotFix
{
    public class LoginSceneEntity : BaseSceneEntity,
        ILoginPanelsEntity
    {
        public LoginPanelsComponent LoginPanelsComponent { get; private set; }

        public override void InitScene()
        {
            LoginPanelsComponent = Add<LoginPanelsComponent, ILoginPanelsEntity>(this);

            LoginPanelsComponent.OpenPanel(LoginPanelsEnum.LoginPanel);
        }

    }
}
