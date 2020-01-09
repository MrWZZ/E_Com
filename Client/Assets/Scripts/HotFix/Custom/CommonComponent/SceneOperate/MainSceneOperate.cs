using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HotFix
{
    public class MainSceneOperate : ISceneOperate
    {
        public SceneEnum SceneType { get; private set; }
        private MainSceneEntity sceneEntity;
        private GameObject root;

        public MainSceneOperate(SceneEnum sceneType)
        {
            SceneType = sceneType;
        }

        public IEnumerator LoadResources()
        {
            yield return 0;

            //加载资源
            root = Resources.Load<GameObject>(SceneType.ToString());

        }

        public void InitScene(Scene gameScene)
        {
            //组装场景
            var rootObj = UnityEngine.Object.Instantiate(root);
            sceneEntity = rootObj.AddComponent<MainSceneEntity>();
            SceneManager.MoveGameObjectToScene(rootObj, gameScene);
            //激活脚本
            sceneEntity.InitScene();
        }


    }
}
