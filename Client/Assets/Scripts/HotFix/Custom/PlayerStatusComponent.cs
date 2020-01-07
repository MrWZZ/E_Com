using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface IPlayerStatusEntity
    {
        GroupTestComponent GroupTestComponent { get; }
        CharacterController CharacterController { get; }
    }

    public class PlayerStatusComponent : 
        BaseStatusComponent,
        IMoveStatusComponent,
        IFallStatusComponent,
        IIdelStatusComponent,
        IJumpStatusComponent,
        IDodgeStatusComponent
    {

        public float MoveSpeed { get; private set; } = 2f;
        public float LookSpeed { get; private set; } = 1f;
        public float Gravity { get; private set; } = -5f;
        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }
        public float JumpUpSpeed { get; private set; } = 5;
        public float MaxJumpUpHeight { get; private set; } = 1.5f;

        public bool IsJump { get; private set; }
        public bool IsDoubleJump { get; private set; }
        public GameObject ComponentGameObject { get { return gameObject; } }
        public GroupTestComponent GroupTestComponent { get { return Entity.GroupTestComponent; } }
        public CharacterController CharacterController { get { return Entity.CharacterController; } }

        public float DodgeSpeed { get; private set; } = 7f;
        public float MaxDodgeDistance { get; private set; } = 2;


        private IPlayerStatusEntity Entity;

        public PlayerStatusComponent Init(IPlayerStatusEntity entity)
        {
            Entity = entity;

            IdelStatusOperate idelStatus = new IdelStatusOperate(this);
            AddStatus(CharacterStatus.Idel, idelStatus);

            MoveStatusOperate moveStatus = new MoveStatusOperate(this);
            AddStatus(CharacterStatus.Move, moveStatus);

            FallStatusOperate fallStatus = new FallStatusOperate(this);
            AddStatus(CharacterStatus.Fall, fallStatus);

            JumpStatusOpreate jumpStatus = new JumpStatusOpreate(this);
            AddStatus(CharacterStatus.Jump, jumpStatus);

            DodgeStatusOperate dodgeStatus = new DodgeStatusOperate(this);
            AddStatus(CharacterStatus.Dodge, dodgeStatus);

            TryChangeStatus(CharacterStatus.Idel);
            return this;
        }

        public void SetHV(float h,float v)
        {
            Horizontal = h;
            Vertical = v;
        }

        public override bool TryChangeStatus(CharacterStatus statusType)
        {
            #region 玩家自身的特殊状态限制

            switch (curStatusType)
            {
                //闪避状态下不能移动
                case CharacterStatus.Dodge:
                    return CheckTryChangeOnDodgeStatus(statusType);
                case CharacterStatus.Jump:
                    return CheckTryChangeOnJumpStatus(statusType);
                case CharacterStatus.Fall:
                    return CheckTryChangeOnFallStatus(statusType);
                case CharacterStatus.Idel:
                    return CheckTryChangeOnIdelStatus(statusType);
                case CharacterStatus.Move:
                    return CheckTryChangeOnMoveStatus(statusType);
            }

            #endregion

            return base.TryChangeStatus(statusType);
        }

        private bool CheckTryChangeOnJumpStatus(CharacterStatus statusType)
        {
            IsJump = true;
            switch (statusType)
            {
                case CharacterStatus.Jump:
                    if (IsDoubleJump)
                    {
                        return false;
                    }
                    else
                    {
                        IsDoubleJump = true;
                        return ForceReEnterCurStatus();
                    }
                case CharacterStatus.Move:
                case CharacterStatus.Dodge:
                    return false;
            }

            return base.TryChangeStatus(statusType);
        }

        private bool CheckTryChangeOnFallStatus(CharacterStatus statusType)
        {
            switch (statusType)
            {
                case CharacterStatus.Jump:
                    if(IsJump)
                    {
                        if (IsDoubleJump)
                        {
                            return false;
                        }
                        else
                        {
                            IsDoubleJump = true;
                            return base.TryChangeStatus(statusType);
                        }
                    }
                    else
                    {
                        return false;
                    }
                case CharacterStatus.Move:
                case CharacterStatus.Fall:
                case CharacterStatus.Dodge:
                    return false;
            }

            return base.TryChangeStatus(statusType);
        }

        private bool CheckTryChangeOnDodgeStatus(CharacterStatus statusType)
        {
            switch (statusType)
            {
                case CharacterStatus.Move:
                case CharacterStatus.Fall:
                case CharacterStatus.Dodge:
                    return false;
            }

            return base.TryChangeStatus(statusType);
        }

        private bool CheckTryChangeOnMoveStatus(CharacterStatus statusType)
        {
            switch (statusType)
            {
                case CharacterStatus.Move:
                    return false;
            }

            return base.TryChangeStatus(statusType);
        }

        private bool CheckTryChangeOnIdelStatus(CharacterStatus statusType)
        {
            IsJump = false;
            IsDoubleJump = false;

            return base.TryChangeStatus(statusType);
        }
    }
}
