using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public enum JumpEnum
    {
        None,//无跳跃状态
        Up,//上升状态
        Stay,//滞空状态
        Down,//下落状态
    }

    public class MoveController : MonoBehaviour
    {
        public float speed = 2f;
        public float gravity = 4f;
        public float jumpSpeed = 2f;
        public float jumpHeight = 1.5f;

        private float lookSpeed = 1f;
        private float margin = 1;
        private bool isOnGrpup = false;
        private bool isJump = false;
        private JumpEnum jumpType = JumpEnum.None;
        private float tempJumpHeight = 0;
        private CharacterController characterController;

        public void Awake()
        {
            characterController = gameObject.GetComponent<CharacterController>();
            margin = characterController.height / 2 + characterController.skinWidth - characterController.radius + 0.01f;
        }

        // 通过射线检测主角是否落在地面或者物体上  
        void GroupTest()
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.down * ( margin + characterController.radius), Color.red, 1f);
            isOnGrpup = Physics.SphereCast(new Ray(transform.position, Vector3.down), characterController.radius, margin);
        }

        void Update()
        {
            GroupTest();
            // 控制移动  
            float h = Input.GetAxis("Horizontal") * speed;
            float v = Input.GetAxis("Vertical") * speed;

            #region 移动

            Vector3 moveDirection = new Vector3(h, isJump ? jumpSpeed : 0, v);
            if (isOnGrpup) 
            {
                if (jumpType == JumpEnum.Down)
                {
                    moveDirection.y = 0;
                    ResetJump();
                }
                
                // 空格键控制跳跃  
                if (Input.GetButtonDown("Jump"))
                {
                    isJump = true;
                    jumpType = JumpEnum.Up;
                }
            }
            else
            {
                if(isJump)
                {
                    if(jumpType == JumpEnum.Up)
                    {
                        tempJumpHeight += jumpSpeed * Time.deltaTime;
                        moveDirection.y = jumpSpeed;
                    }
                    else
                    {
                        tempJumpHeight -= gravity * Time.deltaTime;
                        moveDirection.y = -gravity;
                    }

                    if(tempJumpHeight >= jumpHeight)
                    {
                        jumpType = JumpEnum.Down;
                    }

                }
                else
                {
                    moveDirection.y = -gravity;
                }
            }

            characterController.Move(moveDirection * Time.deltaTime);

            #endregion

            #region 转向

            //注视旋转
            if(moveDirection != Vector3.zero)
            {
                Vector3 lookPos = transform.position + new Vector3(moveDirection.x, 0, moveDirection.z);
                lookPos = Vector3.Slerp(transform.forward, lookPos, lookSpeed);
                transform.LookAt(lookPos);
            }
 
            #endregion

        }

        private void ResetJump()
        {
            tempJumpHeight = 0;
            isJump = false;
            jumpType = JumpEnum.None;
        }
    }
}
