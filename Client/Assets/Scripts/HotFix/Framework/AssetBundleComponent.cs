using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HotFix
{
    public interface IAssetBundleEntity
    {
        FilePathComponent FilePathComponent { get; }
        AssetBundleComponent AssetBundleComponent { get; }
    }

    public class AssetBundleComponent : BaseComponent<IAssetBundleEntity>
    {
        private AssetBundleManifest AssetBundleManifest;
        private Dictionary<string, AssetBundle> assetbundleDic;
        private Dictionary<string, Dictionary<string, UnityEngine.Object>> bundle_NamePath;

        public override void InitComponent()
        {
            assetbundleDic = new Dictionary<string, AssetBundle>();

            if(!Global.GlobalConfig.isUseAssetBundle)
            {
                bundle_NamePath = new Dictionary<string, Dictionary<string, Object>>();
            }
        }

        // 加载Assetsbundle
        public AssetBundle LoadAssetsBundle(string bundleName)
        {
            if (!Global.GlobalConfig.isUseAssetBundle) { return null; }

            if(assetbundleDic.ContainsKey(bundleName)) { return null; }

            string bundleLocalPath = Entity.FilePathComponent.GetLocalAssetBundlePath(bundleName);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundleLocalPath);

            if(assetBundle != null)
            {
                assetbundleDic.Add(bundleName, assetBundle);
                return assetBundle;
            }
            else
            {
                Log.Error($"AssetBundle加载失败:{bundleName}");
                return null;
            }
        }

        // 卸载Assetsbundle
        public void UnLoadAssetBundle(string bundleName)
        {
            if(!Global.GlobalConfig.isUseAssetBundle) { return; }

            AssetBundle assetBundle = null;
            assetbundleDic.TryGetValue(bundleName, out assetBundle);

            if (assetBundle != null)
            {
                assetBundle.Unload(false);
                assetbundleDic.Remove(bundleName);
            }

        }

        /// <summary>
        /// 加载Asset资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">文件名</param>
        /// <param name="bundleName">包名</param>
        /// <returns></returns>
        public T LoadAsset<T>(string fileName,string bundleName) where T : UnityEngine.Object
        {
            if(!Global.GlobalConfig.isUseAssetBundle)
            {
                return LoadLocalAsset<T>(fileName, bundleName);
            }

            AssetBundle assetbundle;
            T asset = null;
            assetbundleDic.TryGetValue(bundleName, out assetbundle);
            if(assetbundle == null)
            {
                Log.Error($"Assetsbunle不存在：{bundleName}");
                return null;
            }
            else
            {
                asset = assetbundle.LoadAsset<T>(fileName);
            }

            if(asset == null)
            {
                Log.Error($"Assetsbundle({bundleName})中不存在该文件：{fileName}");
                return null;
            }
            else
            {
                return asset;
            }
        }

        // 卸载Asset
        public void UnLoadAsset(string bundleName)
        {
            if(!Global.GlobalConfig.isUseAssetBundle) { return; }

            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 读取本地配置文件
        /// </summary>
        /// <param name="bundleName"></param>
        private void ReadLocalConfig(string bundleName)
        {
#if UNITY_EDITOR
            if (bundle_NamePath.ContainsKey(bundleName)) { return; }

            var rrDic = new Dictionary<string, Object>();
            string configPath = $"{Application.dataPath}/Temp/Configs/{bundleName.ToLower()}ResourceReference.txt";
            if (!File.Exists(configPath))
            {
                Log.Warning($"本地配置文件不存在：{configPath}");
                return;
            }

            string[] contentLines = File.ReadAllLines(configPath);
            foreach (var line in contentLines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                string[] lineArr = line.Split(',');
                Object targetObj = AssetDatabase.LoadAssetAtPath(lineArr[1], typeof(Object));
                if (rrDic.ContainsKey(lineArr[0]))
                {
                    Debug.LogError($"{lineArr[0]},{lineArr[1]}\n资源中存在相同名字的引用,请修改其中的一个。");
                    continue;
                }

                rrDic.Add(lineArr[0], targetObj);
            }

            bundle_NamePath.Add(bundleName, rrDic);
#endif
        }

        /// <summary>
        /// 本地测试时，直接读取项目中的文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        private T LoadLocalAsset<T>(string fileName, string bundleName) where T : UnityEngine.Object
        {
            ReadLocalConfig(bundleName);
            var curDic = bundle_NamePath[bundleName];
            if(curDic == null)
            {
                Log.Error($"未成功加载本地配置文件:{bundleName}");
                return default;
            }

            UnityEngine.Object localObj = null;
            curDic.TryGetValue(fileName, out localObj);
            if(localObj == null)
            {
                Log.Error($"文件加载失败：bundleName({bundleName}),fileName({fileName})");
                return default;
            }

            T loadObj = localObj as T;
            if(loadObj == null)
            {
                Log.Error($"文件转换失败：bundleName({bundleName}),fileName({fileName}),Type({typeof(T)})");
                return default;
            }

            return loadObj;
        }

    }
}
