using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class PlayerEntity : 
        BaseEntity,
        IGroupTestEntity,
        IPlayerKeyboardControlEntity,
        IPlayerStatusEntity,
        ILayerEntity
    {
        public PlayerStatusComponent PlayerStatusComponent { get; private set; }
        public PlayerKeyboardControlComponent PlayerKeyboardControlComponent { get; private set; }
        public CharacterController CharacterController { get; private set; }
        public GroupTestComponent GroupTestComponent { get; private set; }
        public LayerComponent LayerComponent { get; private set; }

        private GameObject groupTestPos;
        public Vector3 GroupTestPosition { get { return groupTestPos.transform.position; } }
        public float GroupTestRadius { get; private set; }
        public int GroupTestTargetLayer { get; private set; }

        public LayerEnum EntityLayer { get { return LayerEnum.Player; } }


        public void Awake()
        {
            InitData();
            AddComponent();
        }

        public void InitData()
        {
            CharacterController = gameObject.GetComponent<CharacterController>();
            groupTestPos = gameObject.transform.Find("GroupTest").gameObject;
            float half = (float)Math.Round(Math.Sqrt(CharacterController.radius * CharacterController.radius / 2), 2);
            GroupTestRadius = half;
            GroupTestTargetLayer = LayerManager.OpenAllLayerExceptTarget(EntityLayer);
        }

        public void AddComponent()
        {
            PlayerKeyboardControlComponent = gameObject.AddComponent<PlayerKeyboardControlComponent>().Init(this);
            PlayerStatusComponent = gameObject.AddComponent<PlayerStatusComponent>().Init(this);
            GroupTestComponent = gameObject.AddComponent<GroupTestComponent>().Init(this);
            LayerComponent = gameObject.AddComponent<LayerComponent>().Init(this);
        }

        public void Update()
        {
            GroupTestComponent.OnUpdate();
            PlayerKeyboardControlComponent.OnUpdate();
            PlayerStatusComponent.OnUpdate();
        }

    }
}
