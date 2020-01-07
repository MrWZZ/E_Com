using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface IBoxGroupTestComponent 
    {
        IGroupTestEntity Entity { get; }
        void SetOnGroup(bool isOnGroup);
    }

    public class BoxGroupTestOperate : IGroupTestOperate
    {
        public IBoxGroupTestComponent Component { get; private set; }
        private Vector3 boxHalfSize;
        public BoxGroupTestOperate(IBoxGroupTestComponent component)
        {
            Component = component;
            boxHalfSize = new Vector3(component.Entity.GroupTestRadius, 0.5f, component.Entity.GroupTestRadius);
        }

        public void OnUpdate()
        {
            
            for (int i = 0; i < 6; i++)
            {
                bool isHit = Physics.CheckBox(Component.Entity.GroupTestPosition, boxHalfSize, Quaternion.Euler(0, 90 - 15 * i, 0), Component.Entity.GroupTestTargetLayer);
                if (isHit)
                {
                    Component.SetOnGroup(true);
                    return;
                }
            }

            Component.SetOnGroup(false);
        }
    }
}
