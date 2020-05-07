using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class MainSceneEntity : BaseSceneEntity
    {

        public override void InitScene()
        {
            GameObject mainPanel = Global.LoadAsset<GameObject>("MainPanel", "main");
            Global.OpenPanel<MainPanelEntity>(mainPanel);
        }
    }
}
