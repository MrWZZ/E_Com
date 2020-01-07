using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public enum CharacterStatus
    {
        None = 0,
        Idel,//空闲
        Move,//移动
        Jump,//跳跃
        Fall,//下落
        Dodge,//闪避
    }

    public class BaseStatusComponent : BaseComponent
    {
        public CharacterStatus curStatusType;
        protected IStatusOperate curStatusOperate;
        protected Dictionary<CharacterStatus, IStatusOperate> statusOpreateDic = new Dictionary<CharacterStatus, IStatusOperate>();
        
        protected void AddStatus(CharacterStatus statusType,IStatusOperate statusOprate)
        {
            if(statusOpreateDic.ContainsKey(statusType))
            {
                Log.Warning($"状态已存在，无法重复添加：{statusType}");
            }
            else
            {
                statusOpreateDic.Add(statusType, statusOprate);
            }
        }

        protected void RemoveStatus(CharacterStatus statusType)
        {
            //todo
        }

        /// <summary>
        /// 强制重新进入当前状态
        /// </summary>
        public bool ForceReEnterCurStatus()
        {
            return ChangeStatus(curStatusType);
        }

        public virtual bool TryChangeStatus(CharacterStatus statusType)
        {
            if(curStatusType == statusType)
            {
                return false;
            }

            if(!statusOpreateDic.ContainsKey(statusType))
            {
                Log.Warning($"状态不存在，无法切换：{statusType}");
                return false;
            }

            return ChangeStatus(statusType); 
        }

        private bool ChangeStatus(CharacterStatus statusType)
        {
            curStatusOperate?.Leave();

            curStatusType = statusType;
            curStatusOperate = statusOpreateDic[statusType];

            curStatusOperate.Enter();

            return true;
        }

        public override void OnUpdate()
        {
            curStatusOperate?.OnUpdate();
        }
    }
}
