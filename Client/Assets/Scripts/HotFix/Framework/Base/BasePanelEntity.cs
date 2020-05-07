namespace HotFix
{
    public abstract class BasePanelEntity : BaseSceneEntity
    {
        public override void InitScene()
        {
            
        }

        public abstract void InitPanel(object args = null);
    }
}
