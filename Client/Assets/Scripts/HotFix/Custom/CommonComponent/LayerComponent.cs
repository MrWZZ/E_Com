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
        private ILayerEntity Entity;

        public LayerComponent Init(ILayerEntity entity)
        {
            Entity = entity;

            LayerManager.SetObjectAndChildsLayer(gameObject, Entity.EntityLayer);

            return this;
        }
    }
}
