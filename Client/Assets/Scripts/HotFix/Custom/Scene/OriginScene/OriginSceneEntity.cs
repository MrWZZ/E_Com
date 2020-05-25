using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class OriginSceneEntity : BaseSceneEntity,
        ICoroutineEntity,
        IOriginPanelsEntity,
        IScenesLoadingProgressEntity
    {
        public CoroutineComponent CoroutineComponent { get; private set; }
        public OriginPanelsComponent OriginPanelsComponent { get; private set; }
        public ScenesLoadingProgressComponent ScenesLoadingProgressComponent { get; private set; }

        public override void InitScene()
        {
            // 这里暂时只加载，不释放
            Global.LoadAssetsBundle("origin");

            // 添加组件
            OriginPanelsComponent = Add<OriginPanelsComponent, IOriginPanelsEntity>(this);
            CoroutineComponent = Add<CoroutineComponent, ICoroutineEntity>(this);
            ScenesLoadingProgressComponent = Add<ScenesLoadingProgressComponent, IScenesLoadingProgressEntity>(this);

            Custom.LoadScene(CustomSceneEnum.Login);
        }

        
    }
}
