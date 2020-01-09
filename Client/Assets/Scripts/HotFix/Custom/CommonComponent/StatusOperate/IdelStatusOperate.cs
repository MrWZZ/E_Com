using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    public interface IIdelStatusComponent
    {
        GroupTestComponent GroupTestComponent { get; }
        bool TryChangeStatus(CharacterStatus status);
    }

    public class IdelStatusOperate : IStatusOperate
    {
        public IIdelStatusComponent Component;

        public IdelStatusOperate(IIdelStatusComponent component)
        {
            Component = component;
        }

        public void Enter()
        {
            
        }

        public void Leave()
        {
            
        }

        public void OnUpdate()
        {
            if(!Component.GroupTestComponent.IsOnGroup)
            {
                Component.TryChangeStatus(CharacterStatus.Fall);
            }
        }
    }
}
