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
    public enum SceneEnum
    {
        None,
        Login,
        Main,
    }

    public interface ISceneEntity
    {

    }

    public class SceneComponent : BaseComponent
    {
        public ISceneEntity Entity { get; private set; }
        public SceneEnum CurrentGameScene { get; private set; }

        private Dictionary<SceneEnum, ISceneOperate> opreateDic = new Dictionary<SceneEnum, ISceneOperate>();

        public SceneComponent Init(ISceneEntity entity)
        {
            Entity = entity;

            RegisterOprate(SceneEnum.Login, new LoginSceneOperate(SceneEnum.Login));
            RegisterOprate(SceneEnum.Main, new MainSceneOperate(SceneEnum.Main));

            return this;
        }

        private void RegisterOprate(SceneEnum sceneEnum, ISceneOperate operate)
        {
            if(opreateDic.ContainsKey(sceneEnum))
            {
                Log.Warning($"{sceneEnum}:已经注册，无法重复注册");
                return;
            }

            opreateDic.Add(sceneEnum, operate);
        }

        public void LoadGameScene(SceneEnum sceneEnum)
        {
            if (CurrentGameScene == sceneEnum)
            {
                Log.Warning($"{sceneEnum}:当前已处于该场景，无法加载");
                return;
            }

            StartCoroutine(AsyncLoadNewScene(sceneEnum));
        }

        private IEnumerator AsyncLoadNewScene(SceneEnum sceneEnum)
        {
            if(!opreateDic.ContainsKey(sceneEnum))
            {
                Log.Warning($"{sceneEnum}:该场景未注册，无法加载");
                yield break;
            }

            //组装新场景
            Scene newScene = SceneManager.CreateScene(sceneEnum.ToString());
            yield return StartCoroutine(opreateDic[sceneEnum].LoadResources());
            //卸载旧场景
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            //激活新场景
            SceneManager.SetActiveScene(newScene);
            opreateDic[sceneEnum].InitScene(newScene);

            CurrentGameScene = sceneEnum;
        }

    }
}
