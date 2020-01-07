using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface ILayerEntity
    {
        LayerEnum EntityLayer { get; }
    }

    public class LayerComponent : BaseComponent
    {
        private ILayerEntity entity;

        public LayerComponent Init(ILayerEntity entity)
        {
            this.entity = entity;

            LayerManager.SetObjectAndChildsLayer(gameObject, this.entity.EntityLayer);

            return this;
        }
    }
}
