using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HotFix
{
    public interface IPanelEntity
    {
        PanelComponent PanelComponent { get; }
    }

    public class PanelComponent : BaseComponent<IPanelEntity>
    {
        public Canvas Canvas { get; private set; }
        public EventSystem EventSystem { get; private set; }

        private Dictionary<UILayerEnum, UILayerData> uiLayerDic;

        public override void InitComponent()
        {
            uiLayerDic = new Dictionary<UILayerEnum, UILayerData>();
            CreateUIGroup();
        }

        private void CreateUIGroup()
        {
            GameObject canvasObj = new GameObject("Canvas");
            DontDestroyOnLoad(canvasObj);
            Canvas = canvasObj.AddComponent<Canvas>();
            Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<GraphicRaycaster>();

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1440, 720);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.matchWidthOrHeight = 1;

            var eventSystemObj = new GameObject("EventSystem");
            DontDestroyOnLoad(eventSystemObj);
            EventSystem = eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();

            CreateUILayer(UILayerEnum.OnlyBottom);
            CreateUILayer(UILayerEnum.Back);
            CreateUILayer(UILayerEnum.Center);
            CreateUILayer(UILayerEnum.Front);
            CreateUILayer(UILayerEnum.OnlyTop);
        }

        /// <summary>
        /// 打开页面
        /// </summary>
        /// <param name="panelPre">这个页面的预制体</param>
        /// <param name="uiLayer">页面所在的层</param>
        /// <param name="args">打开页面传递的参数</param>
        public void OpenPanel<PanelType>(GameObject panelPre, UILayerEnum uiLayer = UILayerEnum.Center, object args = null) where PanelType : BasePanelEntity
        {
            var panelData = uiLayerDic[uiLayer];

            // 最顶层/最底层中，只能存在一个界面
            if (uiLayer == UILayerEnum.OnlyBottom || uiLayer == UILayerEnum.OnlyTop)
            {
                CloseAllLayerPanel(uiLayer);
            }

            // 打开的界面已开启，则先关闭，在重新打开让其置顶
            if (panelData.panelEntitys.ContainsKey(panelPre.name))
            {
                ClosePanel(panelPre.name, uiLayer);
            }

            // 触发页面打开
            var panelObj = Instantiate(panelPre, panelData.uiLayerContainer.transform);
            PanelType panelEntity = Global.AddEntity<PanelType>(panelObj);
            panelData.panelEntitys.Add(panelPre.name, panelEntity);
            panelEntity.InitEntity();
            panelEntity._LoadDependBundle();
            panelEntity.InitScene();
            panelEntity.InitPanel(args);
        }

        /// <summary>
        /// 关闭页面
        /// </summary>
        /// <param name="panelPre">这个页面的预制体</param>
        /// <param name="uiLayer">页面所在的层</param>
        public void ClosePanel(string panelName, UILayerEnum uiLayer = UILayerEnum.Center)
        {
            var panelData = uiLayerDic[uiLayer];
            if (!panelData.panelEntitys.ContainsKey(panelName)) { return; }

            // 触发页面关闭
            panelData.panelEntitys[panelName]._OnUnLoad();

            DestroyImmediate(panelData.panelEntitys[panelName].gameObject);
            panelData.panelEntitys.Remove(panelName);
        }

        /// <summary>
        /// 关闭这个成中的所有界面
        /// </summary>
        /// <param name="uiLayer"></param>
        /// <returns></returns>
        public void CloseAllLayerPanel(UILayerEnum uiLayer)
        {
            var panelData = uiLayerDic[uiLayer];
            foreach (var panels in panelData.panelEntitys)
            {
                ClosePanel(panels.Key, uiLayer);
            }
        }

        /// <summary>
        /// 关闭所有界面
        /// </summary>
        public void CloseAllPanel()
        {
            CloseAllLayerPanel(UILayerEnum.OnlyBottom);
            CloseAllLayerPanel(UILayerEnum.Back);
            CloseAllLayerPanel(UILayerEnum.Center);
            CloseAllLayerPanel(UILayerEnum.Front);
            CloseAllLayerPanel(UILayerEnum.OnlyTop);
        }

        /// <summary>
        /// 获取这个层中已打开界面的名字里列表
        /// </summary>
        /// <param name="uiLayer"></param>
        /// <returns></returns>
        public string[] GetLayerOpenPanelsName(UILayerEnum uiLayer)
        {
            var panelData = uiLayerDic[uiLayer];
            int count = panelData.panelEntitys.Keys.Count;
            var nameArr = new string[count];
            panelData.panelEntitys.Keys.CopyTo(nameArr, 0);
            return nameArr;
        }

        /// <summary>
        /// 获取这个层中最顶部界面的名字
        /// </summary>
        /// <param name="uiLayer"></param>
        /// <returns></returns>
        public string GetLayerTopPanelName(UILayerEnum uiLayer)
        {
            var panelData = uiLayerDic[uiLayer];
            string topName = "";

            int count = panelData.uiLayerContainer.transform.childCount;
            if (count != 0)
            {
                var topPanel = panelData.uiLayerContainer.transform.GetChild(count - 1);
                topName = topPanel.name;
            }

            return topName;
        }

        private void CreateUILayer(UILayerEnum uiLayer)
        {
            var layer = new GameObject(uiLayer.ToString());
            layer.transform.SetParent(Canvas.transform);
            var rect = layer.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var layerData = new UILayerData(layer);
            uiLayerDic.Add(uiLayer, layerData);
        }



        private class UILayerData
        {
            public GameObject uiLayerContainer;
            public Dictionary<string, BasePanelEntity> panelEntitys;

            public UILayerData(GameObject container)
            {
                uiLayerContainer = container;
                panelEntitys = new Dictionary<string, BasePanelEntity>();
            }
        }
    }

    public enum UILayerEnum
    {
        /// <summary>
        /// 最底部（唯一）
        /// </summary>
        OnlyBottom,
        /// <summary>
        /// 后一层
        /// </summary>
        Back,
        /// <summary>
        /// 中间层
        /// </summary>
        Center,
        /// <summary>
        /// 前一层
        /// </summary>
        Front,
        /// <summary>
        /// 最顶层
        /// </summary>
        OnlyTop
    }
}
