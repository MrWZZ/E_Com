using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class MainSceneEntity : 
        BaseEntity,
        IResourcesReferenceEntity,
        ISceneReferenceEntity
    {
        public ResourcesReferenceComponent ResourcesReferenceComponent { get; private set; }
        public SceneReferenceComponent SceneReferenceComponent { get; private set; }

        public TextAsset SceneReferenceConfig => ResourcesReferenceComponent.LoadResources<TextAsset>("MainSceneReference", "Main_Bundle");

        public void Awake()
        {
            InitScene();
        }

        public void InitScene()
        {
            ResourcesReferenceComponent = gameObject.AddComponent<ResourcesReferenceComponent>().Init(this);
            SceneReferenceComponent = gameObject.AddComponent<SceneReferenceComponent>().Init(this);

            GameObject playerPre = ResourcesReferenceComponent.LoadResources<GameObject>("Player", "Main_Bundle");
            GameObject playerPos = SceneReferenceComponent.Get<GameObject>("PlayerPos");

            var playerGo = Instantiate(playerPre);
            playerGo.transform.SetParent(playerPos.transform);
            playerGo.transform.localPosition = Vector3.zero;
            playerGo.AddComponent<PlayerEntity>();
        }
    }
}
