// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: Logger.cs
//  Creator 	:  
//  Date		: 2015-11-6
//  Comment		: 
// ***************************************************************


using System;
using System.Collections.Generic;
using System.Diagnostics;


public class Logger
{
    public static Logger defaultLogger = LoggerFactory.Instance.GetLogger(typeof(Logger));

    private bool isStart = true;
    private Type logType;

    public static List<string> contentList = new List<string>();

    public static string GetUIMessage()
    {
        string retStr = "";

        for (int i = 0, count = contentList.Count; i < count; i++)
        {
            retStr += contentList[i] + "\n";
        }
        return retStr;
    }


    public Logger(Type _logType)
    {
        logType = _logType;
    }


    public bool IsStart
    {
        set
        {
            isStart = value;
        }
        get
        {
            return isStart;
        }
    }


    public Type LogType
    {
        get
        {
            return logType;
        }
    }


    public void Exception(string format, params object[] args)
    {
        StackTrace stack = new StackTrace();
        Debug(stack.ToString());
    }


    public void Error(string format, params object[] args)
    {
        if (isStart == false)
            return;
        UnityEngine.Debug.LogError(string.Format(format, args));
        Print("E", string.Format(format, args));
    }


    public void Warn(string format, params object[] args)
    {
        if (isStart == false)
            return;
        UnityEngine.Debug.LogWarning(string.Format(format, args));
        Print("W", string.Format(format, args));
    }


    public void Info(string format, params object[] args)
    {
        if (isStart == false)
            return;
        UnityEngine.Debug.LogWarning(string.Format(format, args));
        Print("I", string.Format(format, args));
    }


    public void Debug(string format, params object[] args)
    {
        if (isStart == false)
            return;
        Print("D", string.Format(format, args));
    }


    public void Assert(bool value, string format)
    {

    }


    public void Assert(bool value)
    {

    }


    private string FormatMessageHeader()
    {
        return LogType + " | " + DateTime.Now.ToShortTimeString() + " ";
    }


    private void Print(string type, string message)
    {
        string content = FormatMessageHeader() + " [" + type + "] " + message;

        string[] printStrSplit = content.Split('\n');
        for (int i = 0; i < printStrSplit.Length; ++i)
        {
            contentList.Add(printStrSplit[i]);
        }
        while (contentList.Count > 20)
        {
            contentList.RemoveAt(0);
        }

        if (type == "E")
        {
            UnityEngine.Debug.LogError(message);
        }
        else if (type == "W")
        {
            UnityEngine.Debug.LogWarning(message);
        }
        else
        {
            UnityEngine.Debug.Log(message);
        }
    }

    [Conditional("TRACE_ON")]
    public static void Log(string message)
    {
        UnityEngine.Debug.Log(message);
    }
}