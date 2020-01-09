using UnityEngine;

namespace HotFix
{
    public class InitEntity : 
        BaseEntity,
        ISceneEntity
    {
        public static InitEntity Instance { get; private set; }

        public SceneComponent SceneComponent { get; private set; }

        public void Awake()
        {
            Instance = this;
            SceneComponent = gameObject.AddComponent<SceneComponent>().Init(this);

            SceneComponent.LoadGameScene(SceneEnum.Login);
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                SceneComponent.LoadGameScene(SceneEnum.Login);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SceneComponent.LoadGameScene(SceneEnum.Main);
            }
        }


    }

}
