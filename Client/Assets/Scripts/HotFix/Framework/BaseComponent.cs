using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public abstract class BaseComponent : MonoBehaviour
    {
        public virtual void OnUpdate()
        {

        }

        public virtual void OnLateUpdate()
        {

        }
    }
}
