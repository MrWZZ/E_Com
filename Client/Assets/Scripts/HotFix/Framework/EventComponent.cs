using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface IEventEntity
    {
        CoroutineComponent CoroutineComponent { get; }
        EventComponent EventComponent { get; }
    }

    public delegate void EventComponentHandler(object args,object argsEx);
    public class EventComponent : BaseComponent<IEventEntity>
    {
        private Dictionary<string, Dictionary<int, EventComponentHandler>> eventDic;
        private Dictionary<int, List<string>> entityEventDic;

        public override void InitComponent()
        {
            eventDic = new Dictionary<string, Dictionary<int, EventComponentHandler>>();
            entityEventDic = new Dictionary<int, List<string>>();
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="args"></param>
        public void TriggerEvent(string eventName, object args = null,object argsEx = null)
        {
            Dictionary<int, EventComponentHandler> curEventDic;
            eventDic.TryGetValue(eventName, out curEventDic);

            if(curEventDic != null)
            {
                foreach (var handler in curEventDic)
                {
                    handler.Value.Invoke(args, argsEx);
                }
            }
        }

        public void AddEvent(string eventName, BaseSceneEntity entity, EventComponentHandler eventHandler)
        {
            Dictionary<int, EventComponentHandler> curEventDic;
            eventDic.TryGetValue(eventName, out curEventDic);

            // 这个事件还没有被注册过
            if (curEventDic == null)
            {
                curEventDic = new Dictionary<int, EventComponentHandler>();
                eventDic.Add(eventName, curEventDic);
            }

            int entityHashCode = entity.GetHashCode();
            if (curEventDic.ContainsKey(entityHashCode))
            {
                Log.Test($"事件已被注册:{entity.name},{eventHandler.Method.Name}");
                return;
            }
            else
            {
                curEventDic.Add(entityHashCode, eventHandler);

                List<string> curEntityEventList;
                entityEventDic.TryGetValue(entityHashCode, out curEntityEventList);
                if (curEntityEventList == null)
                {
                    curEntityEventList = new List<string>();
                    entityEventDic.Add(entityHashCode, curEntityEventList);
                }

                if(curEntityEventList.Contains(eventName))
                {
                    Log.Test($"事件已被注册:{entity.name},{eventHandler.Method.Name}");
                    return;
                }
                else
                {
                    curEntityEventList.Add(eventName);
                }
            }
            
        }

        public void RemoveEvent(string eventName, BaseSceneEntity entity)
        {
            // 删除事件名称字典
            Dictionary<int, EventComponentHandler> curEventDic;
            eventDic.TryGetValue(eventName, out curEventDic);
            if(curEventDic == null)
            {
                Log.Test($"事件不存在：{eventName}");
                return;
            }
            else
            {
                int entityHashCode = entity.GetHashCode();
                if (curEventDic.ContainsKey(entityHashCode))
                {
                    curEventDic.Remove(entityHashCode);
                }
                else
                {
                    Log.Test($"其尚未监听该事件：{entity.name},{eventName}");
                    return;
                }

                // 删除实体事件字典
                List<string> curEntityEventList;
                entityEventDic.TryGetValue(entityHashCode, out curEntityEventList);
                if (curEntityEventList == null)
                {
                    Log.Test($"实体未监听事件：{entity.name}");
                    return;
                }

                if (curEntityEventList.Contains(eventName))
                {
                    curEntityEventList.Remove(eventName);
                }
                else
                {
                    Log.Test($"实体未监听该事件:{entity.name},{eventName}");
                    return;
                }
            }
        }

        public void RemoveAllEntityEvent(BaseSceneEntity entity)
        {
            int entityHashCode = entity.GetHashCode();
            List<string> curEntityEventList;
            entityEventDic.TryGetValue(entityHashCode, out curEntityEventList);

            if(curEntityEventList != null)
            {
                var eventArr = curEntityEventList.ToArray();
                foreach (var eventName in eventArr)
                {
                    RemoveEvent(eventName, entity);
                }
            }
        }

    }
}
