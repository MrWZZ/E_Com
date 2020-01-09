using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface IGroupTestEntity 
    {
        Vector3 GroupTestPosition { get; }
        float GroupTestRadius { get; }
        int GroupTestTargetLayer { get; }
    }

    public class GroupTestComponent : BaseComponent, IBoxGroupTestComponent
    {
        public bool IsOnGroup { get; private set; }
        public IGroupTestEntity Entity { get; private set; }

        public IGroupTestOperate GroupTestOperate { get; private set; }

        public GroupTestComponent Init(IGroupTestEntity entity)
        {
            Entity = entity;

            GroupTestOperate = new BoxGroupTestOperate(this);

            return this;
        }

        public void OnUpdate()
        {
            Debug.DrawLine(transform.position, transform.position - Vector3.down * 0.5f,Color.red,1f);
            GroupTestOperate.OnUpdate();
        }

        public void SetOnGroup(bool isOnGroup)
        {
            IsOnGroup = isOnGroup;
        }
    }

}
