// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: AssetConfProject.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using System;
using System.Collections.Generic;
using TinyJSON;
using TinyNode = TinyJSON.Node;


public enum AssetDiffType
{
    adtAdded,
    adtDeleted,
    adtModified
};


public enum AssetDownloadState
{
    adsUnStarted,
    adsDownloading,
    adsSuccessed
};


public class AssetConf
{
    public string md5;
    public string path;
    public bool compressed;
    public double size;
    public AssetDownloadState downloadState;
};


public class AssetConfDiff
{
    public AssetConf asset;
    public AssetDiffType type;
};


public class AssetConfVersionInfo
{
    public int major = 0;
    public int minor = 0;   //次版本号代表需要进行包替换
    public int patch = 0;   //补丁版本也需要进行包替换， 但这个一般是用来内部版本迭代替换

    public int resourcePatch = 0;

    public  AssetConfVersionInfo(string versionContent)
    {
        string[] versionAdnResSplit = versionContent.Split('-');
        if (versionAdnResSplit.Length != 2)
            throw new Exception("Version error : " + versionContent);
        resourcePatch = int.Parse(versionAdnResSplit[1]);
        string[] versionSplit = versionAdnResSplit[0].Split('.');
        if (versionSplit.Length != 3)
            throw new Exception("Version error : " + versionContent);

        major = int.Parse(versionSplit[0]);
        minor = int.Parse(versionSplit[1]);
        patch = int.Parse(versionSplit[2]);
    }


    public bool IsUpdatePackage(AssetConfVersionInfo versionInfo)
    {
        if (minor < versionInfo.minor)
            return true;
        return false;
    }


    public bool IsUpdateResource(AssetConfVersionInfo versionInfo)
    {
        if (resourcePatch < versionInfo.resourcePatch)
            return true;
        return false;
    }


    public bool IsUseTempResource(AssetConfVersionInfo versionInfo)
    {
        if (resourcePatch == versionInfo.resourcePatch)
            return true;
        return false;
    }
};


public class AssetConfProject
{
    protected static Logger log = LoggerFactory.GetInstance().GetLogger(typeof(Manifest));


    protected bool loaded = false;
    protected string manifestRoot = "";
    public string packageUrl = "";
    public string remoteManifestUrl = "";
    public string remoteVersionUrl = "";
    public string version = "";
    public Dictionary<string, AssetConf> Assets = new Dictionary<string, AssetConf>();
    protected List<string> SearchPaths = new List<string>();
    public AssetConfVersionInfo ConfVersionInfo { set; get; }
    public string packageBodyURI = LS.StrFromXml("package_body_uri");
    public string updateDesc = LS.StrFromXml("package_update_desc");

    private TinyNode json;


    public void LoadFile(string filePath)
    {
        Clear();
        string content = "";
        if (FileUtility.IsFileExist(filePath))
        {
            content = FileUtility.GetString(filePath);
            if (string.IsNullOrEmpty(content))
            {
                log.Error("Fail to read file -> " + filePath);
                return;
            }
            json = JSON.parse(content);
            if (json == null)
            {
                log.Error("Fail to parse Json -> " + filePath);
            }
        }
    }


    public void LoadAndParseFile(string manifestPath)
    {
        LoadFile(manifestPath);
        if (json != null && json.IsTable())
        {
            manifestRoot = System.IO.Path.GetDirectoryName(manifestPath) + "/"; ;
            ParseManifest(json);
        }
    }


    public Dictionary<string, AssetConfDiff> GenDiff(AssetConfProject b)
    {
        Dictionary<string, AssetConfDiff> diffMap = new Dictionary<string, AssetConfDiff>();
        Dictionary<string, AssetConf> bAssets = b.Assets;
        string key = "";
        AssetConf valueA, valueB;
        Dictionary<string, AssetConf>.Enumerator aAssetsEnumerator = Assets.GetEnumerator();
        while(aAssetsEnumerator.MoveNext())
        {
            key = aAssetsEnumerator.Current.Key;
            valueA = aAssetsEnumerator.Current.Value;
            if (!bAssets.ContainsKey(key))
            {
                AssetConfDiff diff = new AssetConfDiff();
                diff.asset = valueA;
                diff.type = AssetDiffType.adtDeleted;
                diffMap.Add(key, diff);
                continue;
            }

            valueB = bAssets[key];
            if (valueA.md5.Equals(valueB.md5) == false)
            {
                AssetConfDiff diff = new AssetConfDiff();
                diff.asset = valueB;
                diff.type = AssetDiffType.adtModified;
                diffMap.Add(key, diff);
            }
        }
        Dictionary<string, bool> md5 = new Dictionary<string, bool>();
        Dictionary<string, AssetConf>.Enumerator bAssetsEnumerator = bAssets.GetEnumerator();
        while (bAssetsEnumerator.MoveNext())
        {
            key = bAssetsEnumerator.Current.Key;
            valueB = bAssetsEnumerator.Current.Value;

            if (!Assets.ContainsKey(key))
            {
                if (md5.ContainsKey(valueB.md5))
                {
                    continue;
                }
                AssetConfDiff diff = new AssetConfDiff();
                diff.asset = valueB;
                diff.type = AssetDiffType.adtAdded;
                diffMap.Add(key, diff);
                md5.Add(valueB.md5, true);
            }
        }
        return diffMap;
    }


    public void GenResumeAssetsList(string urlPath, ref Dictionary<string, AssetDownloadUnit> units, ref Dictionary<string, double> downloadedSize, ref double totalSize)
    {
        Dictionary<string, AssetConf>.Enumerator assetsEnumerator = Assets.GetEnumerator();
        while (assetsEnumerator.MoveNext())
        {
            AssetConf asset = assetsEnumerator.Current.Value;
            string key = assetsEnumerator.Current.Key;
            if (asset.downloadState != AssetDownloadState.adsSuccessed)
            {
                AssetDownloadUnit unit;
                unit.customId = key;
                unit.srcUrl = urlPath + packageUrl + asset.path;
                unit.storagePath = manifestRoot + AssetUtility.GetNotVersionFileName(asset.path);
                unit.downloaded = asset.size;

                if (asset.downloadState == AssetDownloadState.adsDownloading)
                {
                    unit.resumeDownload = true;
                }
                else
                {
                    unit.resumeDownload = false;
                }

                if (!downloadedSize.ContainsKey(key))
                {
                    downloadedSize.Add(key, 0);
                    totalSize += asset.size;
                }
                units.Add(unit.customId, unit);
            }
        }
    }


    public void PrependSearchPaths()
    {
        SearchPathUtility.AddSearchPath(manifestRoot, true);
        List<string> searchPaths = SearchPathUtility.GetSearchPaths();

        for (int i = searchPaths.Count - 1; i >= 0; i--)
        {
            string path = searchPaths[i];
            if (path.Length > 0 && path[path.Length - 1] != '/')
                path += "/";
            path = manifestRoot + path;
            searchPaths.Insert(0, path);
        }
        SearchPathUtility.SetSearchPaths(searchPaths);
    }


    public void ParseManifest(TinyNode json)
    {
        if (json[AssetConstants.KEY_MANIFEST_URL])
            remoteManifestUrl = json[AssetConstants.KEY_MANIFEST_URL].ToString();
        if (json[AssetConstants.KEY_VERSION_URL])
            remoteVersionUrl = json[AssetConstants.KEY_VERSION_URL].ToString();
        if (json[AssetConstants.KEY_VERSION])
        {
            version = json[AssetConstants.KEY_VERSION].ToString();
            ConfVersionInfo = new AssetConfVersionInfo(version);
        }

        if (json[AssetConstants.KEY_PACKAGE_URL])
        {
            packageUrl = json[AssetConstants.KEY_PACKAGE_URL].ToString();
            if (packageUrl.Length > 0 && packageUrl[packageUrl.Length - 1] != '/')
            {
                packageUrl += "/";
            }
        }

        if (json[AssetConstants.KEY_ASSETS])
        {
            TinyNode assets = json[AssetConstants.KEY_ASSETS];
            if (assets.IsTable())
            {
                Dictionary<string, TinyNode> dict = (Dictionary<string, TinyNode>)assets;
                foreach (var itr in dict)
                {
                    string key = itr.Key;
                    AssetConf asset = ParseAsset(key, itr.Value);
                    this.Assets.Add(key, asset);
                }
            }
        }

        if (json[AssetConstants.KEY_SEARCH_PATHS])
        {
            TinyNode paths = json[AssetConstants.KEY_SEARCH_PATHS];
            if (paths.IsArray())
            {
                for (int i = 0; i < paths.Count; i++)
                {
                    if (paths[i].IsString())
                    {
                        SearchPaths.Add(paths[i].ToString());
                    }
                }
            }
        }
        loaded = true;
    }


    public AssetConf ParseAsset(string path, TinyNode json)
    {
        AssetConf asset = new AssetConf();
        if (json[AssetConstants.KEY_MD5])
            asset.md5 = json[AssetConstants.KEY_MD5].ToString();
        if (json[AssetConstants.KEY_PATH])
            asset.path = json[AssetConstants.KEY_PATH].ToString();
        if (json[AssetConstants.KEY_COMPRESSED])
            asset.compressed = (bool)json[AssetConstants.KEY_COMPRESSED];
        if (json[AssetConstants.KEY_SIZE])
            asset.size = (double)json[AssetConstants.KEY_SIZE];
        else
            asset.compressed = false;
        if (json[AssetConstants.KEY_DOWNLOAD_STATE])
            asset.downloadState = (AssetDownloadState)((int)json[AssetConstants.KEY_DOWNLOAD_STATE]);
        else
            asset.downloadState = AssetDownloadState.adsUnStarted;
        return asset;
    }


    public void Clear()
    {
        if (loaded)
        {
            remoteManifestUrl = "";
            remoteVersionUrl = "";
            version = "";
            Assets.Clear();
            SearchPaths.Clear();
            loaded = false;
        }
    }


    public void SaveToFile(string filepath)
    {
        string json = JSON.stringify(this.json);
        FileUtility.WriteFile(filepath, json);
    }


    public void SetAssetDownloadState(string key, AssetDownloadState state)
    {
        if (!Assets.ContainsKey(key))
        {
            return;
        }
        AssetConf asset = Assets[key];
        asset.downloadState = state;
        if (!json.IsTable()) return;
        if (json[AssetConstants.KEY_ASSETS])
        {
            if (!json[AssetConstants.KEY_ASSETS].IsTable())
                return;
            Dictionary<string, TinyNode> jsonAssets = (Dictionary<string, TinyNode>)json[AssetConstants.KEY_ASSETS];
            foreach (var i in jsonAssets)
            {
                string jkey = i.Key;
                if (jkey != key) continue;
                TinyNode entry = i.Value;
                entry[AssetConstants.KEY_DOWNLOAD_STATE] = TinyNode.NewInt((int)state);
            }
        }
    }


    public bool IsLoaded()
    {
        return loaded;
    }
}
