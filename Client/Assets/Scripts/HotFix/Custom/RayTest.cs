using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HotFix.Custom
{
    public class RayTest : MonoBehaviour
    {
        public float maxDistance = 1f;

        public void Update()
        {
            RaycastHit info;
            Debug.DrawRay(transform.position, Vector3.down, Color.red);
            if( Physics.Raycast(new Ray(transform.position,Vector3.down),out info,maxDistance))
            {
                Debug.Log(info);
            }

        }
    }
}
