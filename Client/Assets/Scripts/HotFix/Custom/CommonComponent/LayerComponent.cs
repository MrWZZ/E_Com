using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public interface ILayerComponent
    {
        LayerEnum EntityLayer { get; }
    }

    public class LayerComponent : BaseComponent
    {
        public ILayerComponent Entity;
        public LayerComponent Init(ILayerComponent entity)
        {
            Entity = entity;
            LayerManager.SetObjectAndChildsLayer(gameObject, Entity.EntityLayer);
            return this;
        }
    }
}
