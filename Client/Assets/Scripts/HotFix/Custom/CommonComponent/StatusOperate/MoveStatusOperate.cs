using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface IMoveStatusComponent 
    {
        float MoveSpeed { get; }
        float LookSpeed { get; }
        float Horizontal { get; }
        float Vertical { get; }
        GameObject ComponentGameObject { get; }
        GroupTestComponent GroupTestComponent { get; }
        CharacterController CharacterController { get; }
        bool TryChangeStatus(CharacterStatus status);
    }

    public class MoveStatusOperate : IStatusOperate
    {
        private IMoveStatusComponent Component;

        public MoveStatusOperate(IMoveStatusComponent component)
        {
            Component = component;
        }

        public void Enter()
        {
            
        }

        public void Leave()
        {
            
        }

        public void OnUpdate()
        {
            Vector3 moveDirection = new Vector3(Component.Horizontal * Component.MoveSpeed, 0, Component.Vertical * Component.MoveSpeed);
            Component.CharacterController.Move(moveDirection * Time.deltaTime);

            Vector3 lookPos = Component.ComponentGameObject.transform.position + new Vector3(moveDirection.x, 0, moveDirection.z);
            lookPos = Vector3.Slerp(Component.ComponentGameObject.transform.forward, lookPos, Component.LookSpeed);
            Component.ComponentGameObject.transform.LookAt(lookPos);

            if(!Component.GroupTestComponent.IsOnGroup)
            {
                Component.TryChangeStatus(CharacterStatus.Fall);
            }
            else
            {
                if(Component.Horizontal == 0 && Component.Vertical == 0)
                {
                    Component.TryChangeStatus(CharacterStatus.Idel);
                }
            }

        }
    }
}
