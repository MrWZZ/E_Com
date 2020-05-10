using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotFix
{
    public class HotFixEntity : 
        BaseEntity,
        IFilePathEntity,
        IAssetBundleEntity,
        ISceneEntity,
        IPanelEntity,
        ICoroutineEntity,
        IEventEntity
    {
        public FilePathComponent FilePathComponent { get; private set; }
        public AssetBundleComponent AssetBundleComponent { get; private set; }
        public SceneComponent SceneComponent { get; private set; }
        public PanelComponent PanelComponent { get; private set; }
        public EventComponent EventComponent { get; private set; }
        public CoroutineComponent CoroutineComponent { get; private set; }

        public void Awake()
        {
            InitEntity();
            Global.AddEntity<OriginSceneEntity>(gameObject).InitScene();
        }

        public override void InitEntity()
        {
            Global._HotFixEntity = this;

            FilePathComponent = Add<FilePathComponent, IFilePathEntity>(this);
            AssetBundleComponent = Add<AssetBundleComponent, IAssetBundleEntity>(this);
            SceneComponent = Add<SceneComponent, ISceneEntity>(this);
            PanelComponent = Add<PanelComponent, IPanelEntity>(this);
            CoroutineComponent = Add<CoroutineComponent, ICoroutineEntity>(this);
            EventComponent = Add<EventComponent, IEventEntity>(this);
        }

    }

}
