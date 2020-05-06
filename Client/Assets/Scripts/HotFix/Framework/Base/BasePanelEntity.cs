using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public abstract class BasePanelEntity : BaseSceneEntity
    {
        public override void InitScene()
        {
            InitPanel();
        }

        public abstract void InitPanel();
    }
}
