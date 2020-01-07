using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface IJumpStatusComponent
    {
        float MaxJumpUpHeight { get; }
        float JumpUpSpeed { get; }
        float Gravity { get; }
        float MoveSpeed { get; }
        float Horizontal { get; }
        float Vertical { get; }
        float LookSpeed { get; }
        GameObject ComponentGameObject { get; }
        GroupTestComponent GroupTestComponent { get; }
        CharacterController CharacterController { get; }
        bool TryChangeStatus(CharacterStatus status);
    }

    public class JumpStatusOpreate : IStatusOperate
    {

        public IJumpStatusComponent Component { get; private set; }

        private float tempJumpHeight = 0f;
        public JumpStatusOpreate(IJumpStatusComponent component)
        {
            Component = component;
        }

        public void Enter()
        {
            tempJumpHeight = 0;
        }

        public void Leave()
        {
            
        }

        public void OnUpdate()
        {

            Vector3 moveDirection = new Vector3(Component.Horizontal * Component.MoveSpeed, Component.JumpUpSpeed, Component.Vertical * Component.MoveSpeed);
            Component.CharacterController.Move(moveDirection * Time.deltaTime);

            if(Component.Horizontal != 0 || Component.Vertical != 0)
            {
                Vector3 lookPos = Component.ComponentGameObject.transform.position + new Vector3(moveDirection.x, 0, moveDirection.z);
                lookPos = Vector3.Slerp(Component.ComponentGameObject.transform.forward, lookPos, Component.LookSpeed);
                Component.ComponentGameObject.transform.LookAt(lookPos);
            }

            tempJumpHeight += Component.JumpUpSpeed * Time.deltaTime;
            if(tempJumpHeight >= Component.MaxJumpUpHeight)
            {
                Component.TryChangeStatus(CharacterStatus.Fall);
            }
        }
    }
}
