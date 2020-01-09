 using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace HotFix
{
    public interface ISceneOperate
    {
        IEnumerator LoadResources();
        void InitScene(Scene gameScene);
    }
}
