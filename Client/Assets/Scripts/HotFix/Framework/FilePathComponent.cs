using UnityEngine;

namespace HotFix
{
    public interface IFilePathEntity
    {
        FilePathComponent FilePathComponent { get; }
    }

    public class FilePathComponent : BaseComponent<IFilePathEntity>
    {
        public string PlatformPath
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        return "jar:file://";
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                        return "";
                    default:
                         return "";
                }
            }
        }


        public override void InitComponent()
        {
            
        }

        public string GetLocalAssetBundlePath(string bundleName)
        {
            return $"{PlatformPath}{Application.streamingAssetsPath}/{bundleName}";
        }
    }
}
