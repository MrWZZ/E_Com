using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface IPlayerKeyboardControlEntity
    {
        GroupTestComponent GroupTestComponent { get; }
        PlayerStatusComponent PlayerStatusComponent { get; }
    }

    public class PlayerKeyboardControlComponent : BaseComponent
    {
        private IPlayerKeyboardControlEntity entity;

        public PlayerKeyboardControlComponent Init(IPlayerKeyboardControlEntity entity)
        {
            this.entity = entity;
            return this;
        }

        public void OnUpdate()
        {
            entity.PlayerStatusComponent.SetHV(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            //移动状态
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W))
            {
                entity.PlayerStatusComponent.TryChangeStatus(CharacterStatus.Move);
            }

            //跳跃状态
            if (Input.GetKeyDown(KeyCode.Space))
            {
                entity.PlayerStatusComponent.TryChangeStatus(CharacterStatus.Jump);
            }

            //闪避状态
            if(Input.GetKeyDown(KeyCode.LeftShift))
            {
                entity.PlayerStatusComponent.TryChangeStatus(CharacterStatus.Dodge);
            }

        }

    }
}
