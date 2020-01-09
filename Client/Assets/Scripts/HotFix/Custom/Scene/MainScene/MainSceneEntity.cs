using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class MainSceneEntity : BaseEntity
    {
        public void InitScene()
        {
            var playerPos = gameObject.transform.Find("PlayerPos");
            var playerPre = Resources.Load<GameObject>("Player");
            var playerGo = Instantiate(playerPre);
            playerGo.transform.position = playerPos.transform.position;
            playerGo.AddComponent<PlayerEntity>();
        }
    }
}
