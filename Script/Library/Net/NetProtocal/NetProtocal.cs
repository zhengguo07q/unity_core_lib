// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetProtocal.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using Mono.Xml;
using SLua;
using System;
using System.Collections.Generic;
using System.Security;
using UnityEngine;


[CustomLuaClass]
public class NetProtocal
{
    public Dictionary<short, ProtocolModule> modules = new Dictionary<short, ProtocolModule>();
    public Dictionary<int, ProtocolCommand> command = new Dictionary<int, ProtocolCommand>();

    public void ReadProtocalStr(string protocalStr)
    {
        try
        {
            SecurityParser parser = new SecurityParser();
            parser.LoadXml(protocalStr);
            SecurityElement xmlElement = parser.ToXml();
            if (xmlElement.Tag == ProtocolModule.TAG)
            {
                ProtocolModule module = new ProtocolModule(xmlElement);
                modules.Add(module.value, module);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("parse protocal file error : " + protocalStr + "  " + e.ToString());
        }
    }


    public ProtocolCommand GetCommand(short moduleId, short commandId)
    {
        ProtocolCommand command = null;
        ProtocolModule module = null;

        if (modules.TryGetValue(moduleId, out module))
        {
            module.commands.TryGetValue(commandId, out command);
        }
        return command;
    }


    public static NetProtocal Instance = new NetProtocal();
}

