// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: QualitySetting.cs
//  Creator 	:  
//  Date		: 2015-11-6
//  Comment		: 
// ***************************************************************


using UnityEngine;


public class QualitySetting : SingletonMono<QualitySetting>
{
    public static Logger logger;
    public override void Initialize()
    {
        logger = LoggerFactory.Instance.GetLogger(typeof(QualitySetting));

        int qualityLevel = QualitySettings.GetQualityLevel();
#if UNITY_ANDROID

        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 0;

        if (qualityLevel == 0)
        {
            QualitySettings.shadowCascades = 0;
            QualitySettings.shadowDistance = 15;
        }
        else if (qualityLevel == 5)
        {
            QualitySettings.shadowCascades = 2;
            QualitySettings.shadowDistance = 70;
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        logger.Debug("Android platform quality setting：" + " Application.targetFrameRate = " + Application.targetFrameRate  + "QualitySettings.vSyncCount = " + QualitySettings.vSyncCount + "qualityLevel = " + QualitySettings.GetQualityLevel() );
#endif



#if UNITY_STANDALONE_WIN
         
         Application.targetFrameRate = 60;
         QualitySettings.vSyncCount = 1; 
         
         if (qualityLevel == 0)
         {
             QualitySettings.antiAliasing = 0;
         }
         
         if (qualityLevel == 5)
         {
             QualitySettings.antiAliasing = 8;
         }
                Debug.Log("windows平台质量设置："
        + "Application.targetFrameRate = " + Application.targetFrameRate + "\n"
        + "QualitySettings.vSyncCount = " + QualitySettings.vSyncCount + "\n"
        + "qualityLevel = " + QualitySettings.GetQualityLevel() + "\n"
        );
#endif
    }


}
