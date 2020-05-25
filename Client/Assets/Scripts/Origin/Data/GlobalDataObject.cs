using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataObject/Global")]
public class GlobalDataObject : ScriptableObject
{
    public bool isOnLine;
    public bool isUseDll;
    public bool isUseAssetBundle;
    public bool isShowTestDebug;
    public string serverInitUrl;
    public string clientVersion;
    public string scriptVersion;
    public string assetBundleManifestName;
}
