using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface ICoroutineEntity
    {
        CoroutineComponent CoroutineComponent { get; }
    }

    public class CoroutineComponent : BaseComponent<ICoroutineEntity>
    {
        List<CorData> corDataPool;
        List<Dictionary<int, CorData>> corListPool;
        Dictionary<string, Dictionary<int, CorData>> corlistDic;
        public Dictionary<string, bool> ignoreTipCorDic;

        public class CorData
        {
            public Coroutine cor;
            public Action onComplete;
            public Action onCancel;
        }

        public override void InitComponent()
        {
            corDataPool = new List<CorData>();
            corListPool = new List<Dictionary<int, CorData>>();
            corlistDic = new Dictionary<string, Dictionary<int, CorData>>();
            ignoreTipCorDic = new Dictionary<string, bool>();
        }

        //停止此管理器下的所有协程
        public void Clear()
        {
            foreach (var tokenDic in corlistDic)
            {
                foreach (var corDic in tokenDic.Value)
                {
                    if (corDic.Value.cor != null)
                    {
                        StopCoroutine(corDic.Value.cor);
                    }
                    if (corDic.Value.onCancel != null)
                    {
                        corDic.Value.onCancel.Invoke();
                    }
                }
            }
            corlistDic.Clear();
        }

        /// <summary>
        /// 统一控制协程开启
        /// </summary>
        /// <param name="corName">类型类型标识</param>
        /// <param name="ie">迭代器</param>
        /// <param name="canMutil">是否允许同时开启多个相同标识的协程</param>
        /// <param name="OnComplete"></param>
        /// <param name="onCancel"></param>
        public Coroutine EStartCoroutine(string token, IEnumerator ie, CorMutilEnum mutil = CorMutilEnum.Break, Action onComplete = null, Action onCancel = null)
        {
            if (corlistDic.ContainsKey(token))
            {
                Dictionary<int, CorData> list;
                switch (mutil)
                {
                    case CorMutilEnum.Break:
                        {
                            var pair = corlistDic[token];
                            var firstKey = pair.Keys.ToArray()[0];
                            var corData = pair[firstKey];
                            return corData.cor;
                        }
                    case CorMutilEnum.Cover:
                        {
     
                            EStopCoroutine(token);

                            list = PopCorListItem();
                            var corData = PopCorData();
                            var guid = corData.GetHashCode();
                            corData.onComplete = onComplete;
                            corData.onCancel = onCancel;
                            list.Add(guid, corData);
                            corlistDic.Add(token, list);

                            var corCover = StartCoroutine(CorOperate(token, guid, ie));
                            corData.cor = corCover;

                            return corData.cor;
                        }
                    case CorMutilEnum.Mutil:
                        {
                            list = corlistDic[token];

                            var corData = PopCorData();
                            var guid = corData.GetHashCode();
                            corData.onComplete = onComplete;
                            corData.onCancel = onCancel;
                            list.Add(guid, corData);

                            var corMutil = StartCoroutine(CorOperate(token, guid, ie));
                            corData.cor = corMutil;

                            return corData.cor;
                        }
                    default:
                        return null;

                }
            }
            else
            {
                var list = PopCorListItem();
                var corData = PopCorData();
                var guid = corData.GetHashCode();
                corData.onComplete = onComplete;
                corData.onCancel = onCancel;
                list.Add(guid, corData);
                corlistDic.Add(token, list);

                var corNew = StartCoroutine(CorOperate(token, guid, ie));
                corData.cor = corNew;

                return corData.cor;
            }


        }

        /// <summary>
        /// 停止所有该标识下的协程
        /// </summary>
        /// <param name="token"></param>
        public void EStopCoroutine(string token)
        {
            if (corlistDic.ContainsKey(token))
            {
                var list = corlistDic[token];
                foreach (var item in list)
                {
                    if (item.Value.onCancel != null)
                    {
                        item.Value.onCancel.Invoke();
                    }
                    PushCorData(item.Value);
                }
                PushCorListItem(list);
                corlistDic.Remove(token);
            }
        }

        private CorData PopCorData()
        {
            if (corDataPool.Count > 0)
            {
                var item = corDataPool[0];
                corDataPool.RemoveAt(0);
                return item;
            }
            else
            {
                return new CorData();
            }
        }

        private void PushCorData(CorData data)
        {
            if (data.cor != null)
            {
                StopCoroutine(data.cor);
                data.cor = null;
            }
            data.onCancel = null;
            data.onComplete = null;
            corDataPool.Add(data);
        }

        private Dictionary<int, CorData> PopCorListItem()
        {
            if (corListPool.Count > 0)
            {
                var item = corListPool[0];
                corListPool.RemoveAt(0);
                return item;
            }
            else
            {
                return new Dictionary<int, CorData>();
            }
        }

        private void PushCorListItem(Dictionary<int, CorData> list)
        {
            list.Clear();
            corListPool.Add(list);
        }

        private IEnumerator CorOperate(string token, int guid, IEnumerator ie)
        {
            while (true)
            {
                if (ie != null && ie.MoveNext())
                {
                    yield return ie.Current;
                }
                else
                {
                    if (corlistDic.ContainsKey(token))
                    {
                        var list = corlistDic[token];
                        var data = list[guid];
                        if (data.onComplete != null)
                        {
                            data.onComplete.Invoke();
                        }
                        PushCorData(data);
                        list.Remove(guid);

                        if (list.Count <= 0)
                        {
                            PushCorListItem(list);
                            corlistDic.Remove(token);
                        }
                    }

                    yield break;
                }
            }
        }


    }

    public enum CorMutilEnum
    {
        /// <summary>
        /// 如果已有相同标识的协程正在运行，则忽略本次操作
        /// </summary>
        Break,
        /// <summary>
        /// 如果已有相同标识的协程正在运行，则停止原有协程并运行本次操作
        /// </summary>
        Cover,
        /// <summary>
        /// 可同时运行多个相同标识的协程
        /// </summary>
        Mutil,
    }
}
