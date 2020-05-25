using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    public interface IBaseSceneEntity
    {
        void AddEvent(string eventName, EventComponentHandler eventHandler);
        void TriggerEvent(string eventName, object args = null,object argsEx = null);
    }

    public abstract class BaseSceneEntity : BaseEntity, IBaseSceneEntity
    {
        private Dictionary<string, List<GameObject>> sceneReferenceDic;
        private List<string> dependBundleList;

        public override void InitEntity()
        {
            GetSceneReference();
        }

        public abstract void InitScene();

        public T GetSR<T>(string srName) where T : Component
        {
            List<GameObject> sr = null;
            sceneReferenceDic.TryGetValue(srName, out sr);
            if(sr == null || sr.Count == 0)
            {
                Log.Warning($"获取引用失败：{srName}");
                return null;
            }

            T component = sr[0].GetComponent<T>();
            if(component == null)
            {
                Log.Warning($"物体({sr[0].name})上没有{typeof(T)}类型");
                return null;
            }

            return component;
        }

        public List<GameObject> GetListSR(string srName)
        {
            List<GameObject> sr = null;
            sceneReferenceDic.TryGetValue(srName, out sr);
            if (sr == null)
            {
                Log.Warning($"列表为空：{srName}");
                sr = new List<GameObject>();
            }

            return sr;
        }

        public Button SetClick(string srName, UnityEngine.Events.UnityAction onClick)
        {
            Button button = GetSR<Button>(srName);
            button?.onClick.AddListener(onClick);
            return button;
        }

        private void GetSceneReference()
        {
            sceneReferenceDic = new Dictionary<string, List<GameObject>>();
            SceneReferenceUnit[] sceneReferenceUnits = GetComponentsInChildren<SceneReferenceUnit>(true);
            foreach (var unit in sceneReferenceUnits)
            {
                List<GameObject> sr = null;
                sceneReferenceDic.TryGetValue(unit.name, out sr);

                if (sr == null)
                {
                    sr = new List<GameObject>();
                    sceneReferenceDic.Add(unit.name, sr);
                }
                sr.Add(unit.gameObject);
            }
        }

        /// <summary>
        /// 加载场景所依赖的包
        /// </summary>
        public void _LoadDependBundle()
        {
            var rr = GetComponent<ResourceReference>();
            if(rr == null)
            {
                dependBundleList = new List<string>();
                return;
            }
            dependBundleList = rr.dependBundleList;
            foreach (var bundleName in dependBundleList)
            {
                Global.LoadAssetsBundle(bundleName);
            }
        }

        /// <summary>
        /// 卸载场景所依赖的包
        /// </summary>
        private void _UnLoadDependBundle()
        {
            if(dependBundleList.Count > 0)
            {
                foreach (var bundleName in dependBundleList)
                {
                    Global.UnLoadAssetsBundle(bundleName);
                }
            }
        }

        /// <summary>
        /// 触发关闭操作
        /// </summary>
        public void _OnUnLoad()
        {
            _UnLoadDependBundle();
            RemoveAllEntityEvent();
        }

        public void TriggerEvent(string eventName, object args = null,object argsEx = null)
        {
            Global.TriggerEvent(eventName, args, argsEx);
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventHandler"></param>
        public void AddEvent(string eventName, EventComponentHandler eventHandler)
        {
            Global.AddEvent(eventName, this, eventHandler);
        }

        /// <summary>
        /// 删除指定事件
        /// </summary>
        /// <param name="eventName"></param>
        public void RemoveEvent(string eventName)
        {
            Global.RemoveEvent(eventName,this);
        }

        /// <summary>
        /// 删除所有事件
        /// </summary>
        public void RemoveAllEntityEvent()
        {
            Global.RemoveAllEntityEvent(this);
        }

    }
}
