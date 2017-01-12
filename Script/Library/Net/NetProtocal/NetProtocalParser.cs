// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetProtocalParser.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SLua;


public class NetProtocalParser
{
    private Dictionary<string, List<string>> parser = new Dictionary<string, List<string>>();

    private Dictionary<int, string> resolver = new Dictionary<int, string>();

    public ProtocolCommand Decode(byte[] data, out int id)
    {
        short module = System.BitConverter.ToInt16(data, 0);
        short cmd = System.BitConverter.ToInt16(data, 2);

        ProtocolCommand command = NetProtocal.Instance.GetCommand(module, cmd);
        id = module * 10000 + cmd;
        int idx = 4;
        if (command == null)
        {
            Debug.LogError("find protocal error : " + id);
            return null;
        }

        LuaTable commandTable;
        command = DecodeData(command, data, ref idx, out commandTable) as ProtocolCommand;
        command.table = commandTable;
        return command;
    }


    public ProtocolInterface DecodeData(ProtocolInterface protocolStruct, byte[] data, ref int index, out LuaTable retTable, bool checkStatus = false)
    {
        retTable = new LuaTable(LuaState.main);
        ProtocolCommand command = protocolStruct as ProtocolCommand;
        if (command != null)
        {
            retTable["status"] = System.BitConverter.ToInt16(data, index);  //协议命名这里有特殊的状态
            index += 2;
        }

        Dictionary<string, ProtocolParam> paramDict = protocolStruct.GetParameters();

        foreach (KeyValuePair<string, ProtocolParam> paramPair in paramDict)
        {
            ProtocolParam param = paramPair.Value;
            switch (param.type)
            {
                case ProtocolParamType.pptByte:
                    retTable[param.Name] = data[index];
                    index += 1;
                    break;
                case ProtocolParamType.pptShort:
                    retTable[param.Name] = BitConverter.ToInt16(data, index);
                    index += 2;
                    break;
                case ProtocolParamType.pptInt:
                    retTable[param.Name] = BitConverter.ToInt32(data, index);
                    index += 4;
                    break;
                case ProtocolParamType.pptLong:
                    retTable[param.Name] = BitConverter.ToInt64(data, index);
                    index += 8;
                    break;
                case ProtocolParamType.pptFloat:
                    retTable[param.Name] = BitConverter.ToSingle(data, index);
                    index += 4;
                    break;
                case ProtocolParamType.pptDouble:
                    retTable[param.Name] = BitConverter.ToDouble(data, index);
                    index += 8;
                    break;
                case ProtocolParamType.pptString:
                    StringBuilder str = new StringBuilder();
                    char c = BitConverter.ToChar(data, index);
                    while (c != 0)
                    {
                        str.Append(c);
                        index += 2;
                        c = BitConverter.ToChar(data, index);
                    }
                    if (c == 0) index += 2;
                    retTable[param.Name] = str.ToString();
                    break;
                case ProtocolParamType.pptArray:
                    LuaTable arrayTable = new LuaTable(LuaState.main);
                    int len = BitConverter.ToInt16(data, index);
                    index += 2;
                    if (len > 0)
                    {
                        for (int i = 1; i <= len; i++)
                        {
                            LuaTable subRetTable ;
                            DecodeData(param, data, ref index, out subRetTable);
                            arrayTable[i] = subRetTable;
                        }
                    }
                    retTable[param.Name] = arrayTable;
                    break;
            }
        }

        return protocolStruct;
    }


    public byte[] Encode(LuaTable data, int id)
    {
        byte[] buff = new byte[1024];
        int index = 0;

        short moduleId = (short)(id/10000);
        short commandId = (short)(id%10000);

        WriteToBuff(buff, ref index, moduleId, "pptShort");
        WriteToBuff(buff, ref index, commandId, "pptShort");

        ProtocolCommand command = NetProtocal.Instance.GetCommand(moduleId, commandId);
        EncodeData(buff, ref index, data, command, false);

        byte[] binary = new byte[index];
        Buffer.BlockCopy(buff, 0, binary, 0, index);
        return binary;
    }


    private void EncodeData(byte[] buff, ref int index, object data, ProtocolInterface protocolStruct, bool isList = false)
    {
        LuaTable table = data as LuaTable;
        if (isList)
        {
            short length = 0;
            foreach (LuaTable.TablePair tablePair in table)
            {
                length++;
            }

            Debug.Log("数组长度:" + length);
            WriteToBuff(buff, ref index, length, "short");
            for (var i = 0; i < length; i++) EncodeData(buff, ref index, table[i], protocolStruct, false);
        }
        else
        {

            foreach (KeyValuePair<string, ProtocolParam> paramPair in protocolStruct.GetParameters())
            {
                ProtocolParam paramVal = paramPair.Value;
                paramVal.Value = table[paramPair.Key];
                if (paramVal.Value == null)
                {
                    throw new Exception("parse protocol exception , null param :" + paramVal.Name);
                }

                switch (paramVal.type)
                {
                    case ProtocolParamType.pptString:
                        byte[] bytes = System.Text.Encoding.Unicode.GetBytes(paramVal.Value as string);
                        Buffer.BlockCopy(bytes, 0, buff, index, bytes.Length);
                        index += bytes.Length;
                        WriteToBuff(buff, ref index, (short)0, "pptShort");
                        break;
                    case ProtocolParamType.pptArray:
                        EncodeData(buff, ref index, paramVal.Value, paramVal, true);
                        break;
                    default:
                        string typeContent = paramVal.type.ToString();
                        WriteToBuff(buff, ref index, paramVal.Value, typeContent);
                        break;

                }

            }
        }
    }


    private bool WriteToBuff(Byte[] buff, ref int index, object value, string type)
    {
        byte[] bytes = GetBytes(value, type);
        if (bytes == null) return false;

        Buffer.BlockCopy(bytes, 0, buff, index, bytes.Length);
        index += bytes.Length;
        return true;
    }


    private byte[] GetBytes(object obj, string type)
    {
        byte[] data = null;
        switch (type)
        {
            case "pptByte": data = new byte[1]; data[0] = Convert.ToByte(obj); break;
            case "pptShort": data = BitConverter.GetBytes(Convert.ToInt16(obj)); break;
            case "pptInt": data = BitConverter.GetBytes(Convert.ToInt32(obj)); break;
            case "pptLong": data = BitConverter.GetBytes(Convert.ToInt64(obj)); break;
            case "pptFloat": data = BitConverter.GetBytes(Convert.ToSingle(obj)); break;
            case "pptDouble": data = BitConverter.GetBytes(Convert.ToDouble(obj)); break;
        }
        return data;
    }


    public static NetProtocalParser Instance = new NetProtocalParser();
}


