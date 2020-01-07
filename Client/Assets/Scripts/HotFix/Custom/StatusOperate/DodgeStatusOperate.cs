using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface IDodgeStatusComponent
    {
        float DodgeSpeed { get; }
        float MaxDodgeDistance { get; }
        float Horizontal { get; }
        float Vertical { get; }
        GameObject ComponentGameObject { get; }
        GroupTestComponent GroupTestComponent { get; }
        CharacterController CharacterController { get; }
        bool TryChangeStatus(CharacterStatus status);
    }

    public class DodgeStatusOperate : IStatusOperate
    {
        public IDodgeStatusComponent Component { get; private set; }

        private float tempDodgeDistance = 0;
        private Vector3 tempDodgeDirection = Vector3.zero;
        public DodgeStatusOperate(IDodgeStatusComponent component)
        {
            Component = component;
        }

        public void Enter()
        {
            tempDodgeDistance = 0;
            if (Component.Horizontal == 0 && Component.Vertical == 0)
            {
                //默认朝实体前方滚动
                tempDodgeDirection = Component.ComponentGameObject.transform.forward;
            }
            else
            {
                float h = 0;
                float v = 0;
                if(Component.Horizontal > 0)
                {
                    h = 1;
                }
                else if(Component.Horizontal == 0)
                {
                    h = 0;
                }
                else
                {
                    h = -1;
                }

                if (Component.Vertical > 0)
                {
                    v = 1;
                }
                else if (Component.Vertical == 0)
                {
                    v = 0;
                }
                else
                {
                    v = -1;
                }

                tempDodgeDirection = new Vector3(h, 0, v);
            }
        }

        public void Leave()
        {
            
        }

        public void OnUpdate()
        {
            Component.CharacterController.Move(tempDodgeDirection * Component.DodgeSpeed * Time.deltaTime);

            tempDodgeDistance += Component.DodgeSpeed * Time.deltaTime;

            if (tempDodgeDistance >= Component.MaxDodgeDistance)
            {
                Component.TryChangeStatus(CharacterStatus.Idel);
            }
        }
    }
}
