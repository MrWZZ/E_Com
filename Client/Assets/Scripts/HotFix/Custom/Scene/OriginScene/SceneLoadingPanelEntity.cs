using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix
{
    public class SceneLoadingPanelEntity : BasePanelEntity
    {
        private Text progressText;
        public override void InitPanel(object args = null)
        {
            progressText = GetSR<Text>("progressText");
        }

        public void SetProgressText(float progress)
        {
            progressText.text = $"加载中...{Mathf.FloorToInt(progress * 100)}%";
        }
    }
}
