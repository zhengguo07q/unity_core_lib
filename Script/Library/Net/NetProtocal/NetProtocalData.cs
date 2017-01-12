// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetProtocalData.cs
//  Creator 	:  
//  Date		: 2016-12-2
//  Comment		: 
// ***************************************************************


using SLua;
using System;
using System.Collections.Generic;
using System.Security;

public enum ProtocolParamType
{
    pptNull,
    pptByte,
    pptShort,
    pptInt,
    pptLong,
    pptFloat,
    pptDouble,
    pptString,
    pptArray
}


public enum ProtocolCommandDirection
{
    pcdClient,
    pcdServer
}

//一个命令可能含有参数， 也可能没有， 有可能有多个参数， 而且参数可能是列表

public abstract class ProtocolInterface
{
    public string Name;
    public System.Object Value;
    public abstract Dictionary<string, ProtocolParam> GetParameters();
}


public class ProtocolParam : ProtocolInterface
{
    public ProtocolParamType type = ProtocolParamType.pptNull;
    public int length;
    public string remark;
    public string content;

    public Dictionary<string, ProtocolParam> paramList;


    public ProtocolParam(SecurityElement element)
    {
        Name = element.Attribute("name");

        string typeContent = element.Attribute("type");
        string enumTypeContent = "ppt" + typeContent[0].ToString().ToUpper() + typeContent.Substring(1);

        if (Enum.IsDefined(typeof(ProtocolParamType), enumTypeContent))
        {
            type = (ProtocolParamType)Enum.Parse(typeof(ProtocolParamType), enumTypeContent);
        }
        else
        {
            type = ProtocolParamType.pptArray;
        }

        if (type == ProtocolParamType.pptArray && element.Children != null)
        {
            paramList = new Dictionary<string, ProtocolParam>();
            foreach (SecurityElement child in element.Children)
            {
                ProtocolParam param = new ProtocolParam(child);
                paramList.Add(param.Name, param);
            }
        }

        length = int.Parse(element.Attribute("size"));
        remark = element.Attribute("remark");
        content = element.Text;
    }


    public override Dictionary<string, ProtocolParam> GetParameters()
    {
        return paramList;
    }

}


public class ProtocolCommand : ProtocolInterface
{
    public static string TAG = "command";
    public string info;
    public ProtocolCommandDirection direction;
    private Dictionary<string, ProtocolParam> paramPros = new Dictionary<string, ProtocolParam>();
    public short status = -1;
    private ProtocolModule module;
    private int commandId;

    public LuaTable table;

    public ProtocolCommand(ProtocolModule _module, SecurityElement element)
    {
        module = _module;
        Name = element.Attribute("cmd_name");
        info = element.Attribute("info");
        Value = short.Parse(element.Attribute("value"));
        direction = (ProtocolCommandDirection)Enum.Parse(typeof(ProtocolCommandDirection), "pcd" + element.Attribute("direction"));
        if (element.Children != null)
        {
            foreach (SecurityElement child in element.Children)
            {
                ProtocolParam param = new ProtocolParam(child);
                paramPros.Add(param.Name, param);
            }
        }

        commandId = module.value * 10000 + short.Parse(Value.ToString());
    }


    public override Dictionary<string, ProtocolParam> GetParameters()
    {
        return paramPros;
    }


    public LuaTable GetLuaProtocol()
    {
        LuaTable protoData = ScriptManager.Instance.Env["protoData"] as LuaTable;
        protoData[commandId] = table;
        return table;
    }

}


public class ProtocolModule
{
    public static string TAG = "module";
    public string name;
    public string proxy;
    public short value;
    public string info;

    public Dictionary<short, ProtocolCommand> commands = new Dictionary<short, ProtocolCommand>();


    public ProtocolModule(SecurityElement element)
    {
        name = element.Attribute("module_name");
        proxy = element.Attribute("proxy");
        value = short.Parse(element.Attribute("value"));
        info = element.Attribute("info");

        if (element.Children != null)
        {
            foreach (SecurityElement child in element.Children)
            {
                if (child.Tag == ProtocolCommand.TAG)
                {
                    ProtocolCommand command = new ProtocolCommand(this, child);
                    commands.Add((short)command.Value, command);
                }
            }
        }
    }
}
