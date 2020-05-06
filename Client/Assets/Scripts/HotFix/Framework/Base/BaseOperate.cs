using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public abstract class BaseOperate
    {
        
    }

    public abstract class BaseOperate<ComponentType> : BaseOperate
    {
        public ComponentType Component { get; private set; }
        public void _Init(ComponentType component)
        {
            Component = component;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public abstract void InitOperate();

    }
}
