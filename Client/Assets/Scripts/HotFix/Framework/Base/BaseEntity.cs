using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public abstract class BaseEntity : MonoBehaviour
    {
        private List<BaseComponent> componentList = new List<BaseComponent>();

        /// <summary>
        /// 初始化
        /// </summary>
        public abstract void InitEntity();

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="ComponentType">组件的类型</typeparam>
        /// <typeparam name="EntityType">组件依附的实体类型</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ComponentType Add<ComponentType, EntityType>(EntityType entity) where ComponentType : BaseComponent<EntityType>
        {
            var component = gameObject.AddComponent<ComponentType>();
            component._Init(entity);
            component.InitComponent();
            componentList.Add(component);
            return component;
        }

        /// <summary>
        /// 摧毁实体
        /// </summary>
        public void DestroyEntity()
        {
            DestroyImmediate(this);
        }
    }
}
