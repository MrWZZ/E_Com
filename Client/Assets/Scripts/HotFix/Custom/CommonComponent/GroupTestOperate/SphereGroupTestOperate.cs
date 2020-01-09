using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface ISphereGroupTestComponent
    {
        IGroupTestEntity Entity { get; }
        void SetOnGroup(bool isOnGroup);
    }

    public class SphereGroupTestOperate : IGroupTestOperate
    {
        public ISphereGroupTestComponent Component { get; private set; }
        public SphereGroupTestOperate(ISphereGroupTestComponent component)
        {
            Component = component;
        }

        public void OnUpdate()
        {
            bool isOnGroup = Physics.CheckSphere(Component.Entity.GroupTestPosition, Component.Entity.GroupTestRadius, Component.Entity.GroupTestTargetLayer);
            Component.SetOnGroup(isOnGroup);
        }
    }
}
