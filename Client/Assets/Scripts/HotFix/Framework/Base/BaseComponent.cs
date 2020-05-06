using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public abstract class BaseComponent : MonoBehaviour
    {

    }

    public abstract class BaseComponent<EntityType> : BaseComponent
    {
        private List<BaseOperate> operateList = new List<BaseOperate>();
        public EntityType Entity { get; private set; }
        public void _Init(EntityType entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public abstract void InitComponent();

        /// <summary>
        /// 添加操作
        /// </summary>
        /// <typeparam name="OperateType">操作类型</typeparam>
        /// <typeparam name="ComponentType">组件类型</typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public OperateType Add<OperateType, ComponentType>(ComponentType component) where OperateType : BaseOperate<ComponentType>, new()
        {
            var operate = new OperateType();
            operate._Init(component);
            operate.InitOperate();
            operateList.Add(operate);
            return operate;
        }
    }
}
