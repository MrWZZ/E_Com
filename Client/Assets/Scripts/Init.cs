using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class Init : MonoBehaviour
{
    public bool HotFix = false;
    public Version newestDllVersion;

    void Awake()
    {
        if (HotFix)
        {
            Debug.Log("on hotfix model");

            StartCoroutine(PostVersion());
        }
        else
        {
            Debug.Log("on common model");

            gameObject.AddComponent<HotFix.InitEntity>();
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 检测版本信息
    /// </summary>
    /// <returns></returns>
    IEnumerator PostVersion()
    {
        TextAsset json = Resources.Load<TextAsset>("version");
        Struct_version data_version = JsonUtility.FromJson<Struct_version>(json.text);

        if (!string.IsNullOrEmpty(data_version.server_init_url))
        {
            WWW result = new WWW(data_version.server_init_url);

            yield return result;

            if (result.error != null)
            {
                Debug.Log("访问失败：" + result.error);
            }
            else
            {
                // 请求成功

                Version clientVersion = new Version(data_version.client_version);
                Version dllVersion = new Version(data_version.dll_version);

                newestDllVersion = dllVersion;
                if (PlayerPrefs.HasKey("dll_version"))
                {
                    Version saveDllVersion = new Version(PlayerPrefs.GetString("dll_version"));
                    newestDllVersion = saveDllVersion > dllVersion ? saveDllVersion : dllVersion;
                }

                Struct_init data = JsonUtility.FromJson<Struct_init>(result.text);
                Version serverClientVersion = new Version(data.client_version);
                Version serverDllVersion = new Version(data.dll_version);

                if(serverClientVersion > clientVersion)
                {
                    Debug.Log("客户端需要更新");
                    //todo 弹窗提示点击跳转链接
                    Application.OpenURL(data.client_url);
                    yield break;
                }

                string localPath = Application.persistentDataPath + Regex.Replace(data.dll_url, ".+?//.+?/", "/");
                
                //服务器版本高则网络加载
                if(serverDllVersion > newestDllVersion)
                {
                    Debug.Log("脚本需要更新");
                    //todo 提示是否要更新

                    newestDllVersion = serverDllVersion;
                    //先删除旧的Dll
                    if (File.Exists(localPath))
                    {
                        //todo 断点续传支持
                        File.Delete(localPath);
                    }
                    StartCoroutine(LoadAssetBundle(data.dll_url, localPath, OnComplete, OnProgress, OnFail));
                }
                else
                {
                    //如果版本和资源中的一样，说明自带的脚本就是最新的
                    if(serverDllVersion == dllVersion)
                    {
                        //否则本地读取
                        StartCoroutine(LoadHotFixDll(Application.streamingAssetsPath + Regex.Replace(data.dll_url, ".+?//.+?/", "/"), OnComplete));
                    }
                    else
                    {
                        //否则本地读取
                        StartCoroutine(LoadHotFixDll(localPath, OnComplete));
                    }
                }
            }
        }
        else
        {
            Debug.LogError("URL不能为空");
        }
    }

    public class Struct_version
    {
        public string server_init_url;
        public string client_version;
        public string dll_version;
    }

    public class Struct_init
    {
        public string client_version;
        public string client_url;
        public string dll_version;
        public string dll_url;
    }

    public void OnComplete(AssetBundle bundle)
    {
        TextAsset asset = bundle.LoadAsset("hotfix", typeof(TextAsset)) as TextAsset;

        var assembly = Assembly.Load(asset.bytes);
        Type type = assembly.GetType("HotFix.InitEntity");
        gameObject.AddComponent(type);

        Debug.Log("cur dll version:" + newestDllVersion.ToString());
        PlayerPrefs.SetString("dll_version", newestDllVersion.ToString());
    }

    public void OnProgress(long curPro,long totalPro)
    {
        Debug.Log($"加载中：{curPro / totalPro * 1.0f}");
    }

    public void OnFail()
    {
        Debug.LogError("加载失败");
    }

    IEnumerator LoadHotFixDll(string dllPath, Action<AssetBundle> completeAction)
    {

        string beforePath = "";
        switch(Application.platform)
        {
            case RuntimePlatform.Android:
                beforePath = "jar:file://";
                break;
            default:
                beforePath = "file:///";
                break;
        }

        dllPath = beforePath + dllPath;
        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(dllPath))
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log($"热更脚本加载出错：{uwr.error}");
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                completeAction?.Invoke(bundle);
            }
        }
    }

    public IEnumerator LoadAssetBundle(string url, string filePath, Action<AssetBundle> completeAction, Action<long, long> progressAction, Action failAction)
    {
        //Head方法可以获取到文件的全部长度
        UnityWebRequest huwr = UnityWebRequest.Head(url);
        yield return huwr.SendWebRequest();
        if (huwr.isNetworkError || huwr.isHttpError)
        {
            Debug.Log(huwr.error);
            failAction?.Invoke();
        }
        else
        {
            //首先拿到文件的全部长度
            long totalLength = long.Parse(huwr.GetResponseHeader("Content-Length"));
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                //当前文件长度
                long nowFileLength = fs.Length;
                if (nowFileLength < totalLength)
                {
                    //设置文件写入位置
                    fs.Seek(nowFileLength, SeekOrigin.Begin);
                    UnityWebRequest uwr = UnityWebRequest.Get(url);

                    //设置从指定位置开始下载
                    uwr.SetRequestHeader("Range", "bytes=" + nowFileLength + "-" + totalLength);
                    uwr.SendWebRequest();

                    //从该索引处继续下载
                    long index = 0;
                    while (!uwr.isDone)
                    {
                        yield return null;
                        byte[] data = uwr.downloadHandler.data;
                        if (data != null)
                        {
                            long length = data.Length - index;
                            //写入文件
                            fs.Write(data, (int)index, (int)length);
                            index += length;
                            nowFileLength += length;
                            progressAction?.Invoke(nowFileLength, totalLength);
                            //如果下载完成了
                            if (nowFileLength >= totalLength)
                            {
                                StartCoroutine(LoadHotFixDll(filePath, OnComplete));
                            }
                        }
                    }

                    if (uwr.isNetworkError || uwr.isHttpError)
                    {
                        failAction?.Invoke();
                        Debug.Log(uwr.error);
                    }
                }
                else
                {
                    StartCoroutine(LoadHotFixDll(filePath,OnComplete));
                }
            }
        }
    }
}
