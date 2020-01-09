using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public enum LayerEnum
    {
        Player = 8,
    }

    public static class LayerManager
    {

        /// <summary>
        /// 将一个游戏物体及所有子物体都设置为某个层
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="layer"></param>
        public static void SetObjectAndChildsLayer(GameObject gameObject,LayerEnum layer)
        {
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
            foreach (var item in transforms)
            {
                item.gameObject.layer = (int)LayerEnum.Player;
            }
        }

        /// <summary>
        /// 打开目标层的射线检测
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static int OpenTargetLayer(LayerEnum layer)
        {
            int layerIndex = (int)layer;

            return 1 << layerIndex;
        }

        /// <summary>
        /// 打开除了目标外的所有层射线检测
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static int OpenAllLayerExceptTarget(LayerEnum layer)
        {
            int layerIndex = (int)layer;

            return ~(1 << layerIndex);
        }

    }
}
