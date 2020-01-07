using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public interface IStatusOperate 
    {
        /// <summary>
        /// 进入状态
        /// </summary>
        void Enter();

        /// <summary>
        /// 离开状态
        /// </summary>
        void Leave();

        /// <summary>
        /// 更新状态
        /// </summary>
        void OnUpdate();
    }
}
