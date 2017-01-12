// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: LoggerReport.cs
//  Creator 	:  
//  Date		: 2015-11-6
//  Comment		: 
// ***************************************************************


using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class LoggerReport : SingletonMono<LoggerReport>
{
    public List<string> logList = new List<string>();
    public float lastSavedTime;
    public string logFile = @"e:\game.log";
    public int maxLogLength = 100000;
    private Socket m_sock;


    override public void Initialize()
    {
        InitLocalFile();
        InitLogNetwork();
    }


    public void InitLocalFile()
    {
        if (Directory.Exists(PathUtility.PersistentDataPath + "/Log") == false)
            Directory.CreateDirectory(PathUtility.PersistentDataPath + "/Log");

        logFile = Path.Combine(PathUtility.PersistentDataPath + "/Log", "Game" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");

        CleanOldFile();

        FileInfo fileInfo = new FileInfo(logFile);

        if (fileInfo.Exists == false || fileInfo.Length > maxLogLength)
        {
            FileUtility.WriteFile(logFile, GetCurrDate());
        }
        else
        {
            using (StreamWriter sw = File.AppendText(logFile))
            {
                sw.WriteLine(GetCurrDate());
                sw.Flush();
                sw.Close();
            }
        }

        Application.logMessageReceived +=Handler;
        InvokeRepeating("SaveLogInfo", 0, 20f);

    }


    private void CleanOldFile()
    {
        string path = PathUtility.PersistentDataPath + "/Log";
        string[] files = Directory.GetFiles(path);
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Length > 15)
            {
                int leght = files[i].Length;
                if (date.Equals(files[i].Substring(leght - 18 + 4, 10)) == false)
                {
                    File.Delete(files[i]);
                }
            }
        }
    }


    private string GetCurrDate()
    {
        return "[ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " ]";
    }


    private void SaveLogInfo()
    {
        WriteInfo();
    }


    private void WriteInfo()
    {
        using (StreamWriter sw = File.AppendText(logFile))
        {
            if (logList.Count > 0)
            {
                for (int i = 0; i < logList.Count; i++)
                    sw.WriteLine(logList[i]);
                sw.Flush();
                sw.Close();

                logList.Clear();
                lastSavedTime = Time.time;
            }
        }
    }


    public string GetLogFileContent()
    {
        using (FileStream sw = File.OpenRead(logFile))
        {
            using (StreamReader streamReader = new StreamReader(sw))
            {
                return streamReader.ReadToEnd();
            }
        }
    }


    public void DeleteFile()
    {
        if (File.Exists(logFile))
            File.Delete(logFile);
    }


    public byte[] GetLogFileByte()
    {
        if (!File.Exists(logFile))
            return null;
        return ReadBytes(logFile);
    }


    public static byte[] ReadBytes(string fileName)
    {
        using (Stream serializeStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            int leng = (int)serializeStream.Length;
            byte[] bytes = new byte[leng];
            serializeStream.Read(bytes, 0, leng);
            return bytes;
        };
    }


    private void Handler(string condition, string stackTrace, LogType type)
    {
        string msg = "";
        if (type == LogType.Warning)
        {
            msg += Time.time + "[Warning]" + "  " + condition;
            return;
        }
        else if (type == LogType.Error)
            msg += Time.time + "[Error]" + "  " + condition;
        else if (type == LogType.Exception)
            msg += Time.time + "[FATAL]" + "  " + condition + " " + stackTrace;
        else
            msg += Time.time + "  " + condition;

        logList.Add(msg);
    }


    private void OnApplicationQuit()
    {
        CancelInvoke("SaveLogInfo");
        WriteInfo();
    }


    public void InitLogNetwork()
    {
        try
        {
            if (m_sock != null && m_sock.Connected)
            {
                m_sock.Shutdown(SocketShutdown.Both);
                System.Threading.Thread.Sleep(10);
                m_sock.Close();
            }

            m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint epServer = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 399);

            m_sock.Blocking = false;
            AsyncCallback onconnect = new AsyncCallback(OnConnect);
            m_sock.BeginConnect(epServer, onconnect, m_sock);
        }
        catch
        {
            msg("Server Connect failed!");
        }
    }


    public void OnConnect(IAsyncResult ar)
    {
        Socket sock = (Socket)ar.AsyncState;

        try
        {
            if (sock.Connected)
                msg("Logger ok!");
            else
                msg("Unable to connect to remote machine.");
        }
        catch
        {
            msg("Unusual error during Connect!");
        }
    }


    private static void msg(string str)
    {
        Debug.Log(str);
    }


    public void Send(string str)
    {
        if (m_sock == null || !m_sock.Connected)
        {
            msg(str);
            return;
        }
        try
        {
            Byte[] byteDateLine = Encoding.ASCII.GetBytes(str.ToCharArray());
            m_sock.Send(byteDateLine, byteDateLine.Length, 0);
        }
        catch
        {
            msg("Send Message Failed!");
        }
    }
}