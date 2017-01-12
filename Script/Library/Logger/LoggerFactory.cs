// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: LoggerFactory.cs
//  Creator 	:  
//  Date		: 2015-11-6
//  Comment		: 
// ***************************************************************


using System;
using System.Collections.Generic;
using System.Linq;


public class LoggerFactory : SingletonMono<LoggerFactory>
{
    public bool IsDefaultStart = true;
    public Dictionary<Type, Logger> logDict;
    public List<Logger> currStartList;
    public List<Type> InitType = null;

    public override void Initialize()
    {
        LoggerReport.GetInstance();
        LoggerView.GetInstance();

        logDict = new Dictionary<Type, Logger>();
        currStartList = new List<Logger>();
        InitType = new List<Type>();
    }


    public Logger GetLogger(Type type)
    {
        Logger log = null;
        if (!logDict.ContainsKey(type))
        {
            log = new Logger(type);
            logDict[type] = log;
        }
        else
        {
            log = logDict[type];
        }
        log.IsStart = IsDefaultStart;
        return log;
    }


    public void StartLog()
    {
    }

    public void StartLog(Type type)
    {
        Logger log = GetLogger(type);
        log.IsStart = true;
        currStartList.Add(log);
    }


    public int StartLog(string typeName)
    {
        int count = 0; //开启的日志数
        Logger log;
        var logDictEt = logDict.GetEnumerator();

        while (logDictEt.MoveNext())
        {
            if (logDictEt.Current.Key.Name.IndexOf(typeName) > 0)
            {
                log = logDictEt.Current.Value;
                log.IsStart = true; //设置此类日志开启
                currStartList.Add(log);
                count++;
            }
        }
        return count;
    }


    public int CloseLog(string typeName)
    {
        int count = 0;
        for (int i = 0; i < currStartList.Count; i++)
        {
            Logger log = currStartList[i];
            if (log.LogType.Name.IndexOf(typeName) > -1)
            {
                log.IsStart = false;
                currStartList.Remove(log);
            }
            count++;
        }
        return count;
    }


    public int CloseAllLog()
    {
        int count = 0;
        for (int i = 0; i < currStartList.Count; i++)
        {
            Logger log = currStartList[i];
            log.IsStart = false;
            count++;
        }
        currStartList = new List<Logger>();
        return count;
    }


    public int Count
    {
        get
        {
            return currStartList.Count();
        }
    }
}