using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class MainSceneEntity : BaseSceneEntity,
        IMainPanelsEntity
    {
        public MainPanelsComponent MainPanelsComponent { get; private set; }

        public override void InitScene()
        {
            MainPanelsComponent = Add<MainPanelsComponent, IMainPanelsEntity>(this);

            TriggerEvent(MainSceneEvent.OpenMainPanel, MainPanelsEnum.MainPanel);
        }
    }
}
