// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: AssetConstants.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class AssetConstants
{
    public const string VERSION_FILENAME = "version.manifest";
    public const string MANIFEST_FILENAME = "project.manifest";
    public const string TEMP_MANIFEST_FILENAME = "project.manifest.temp";
    public const string Manifest_Filename_Remote = "project.manifest.remote";

    public const string VERSION_ID = "@version";
    public const string MANIFEST_ID = "@manifest";
    public const string BATCH_UPDATE_ID = "@batch_update";
    public const int Max_Repeat_Count_Version = 3;
    public const int Max_Repeat_Count_Manifest = 3;
    public const int Max_Repeat_Count_Update = 5;

    public const string KEY_VERSION = "version";
    public const string KEY_PACKAGE_URL = "packageUrl";
    public const string KEY_MANIFEST_URL = "remoteManifestUrl";
    public const string KEY_VERSION_URL = "remoteVersionUrl";
    public const string KEY_ASSETS = "assets";
    public const string KEY_COMPRESSED_FILES = "compressedFiles";
    public const string KEY_SEARCH_PATHS = "searchPaths";

    public const string KEY_PATH = "path";
    public const string KEY_MD5 = "md5";
    public const string KEY_GROUP = "group";
    public const string KEY_COMPRESSED = "compressed";
    public const string KEY_SIZE = "size";
    public const string KEY_COMPRESSED_FILE = "compressedFile";
    public const string KEY_DOWNLOAD_STATE = "downloadState";
}

