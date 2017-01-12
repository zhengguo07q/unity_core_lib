// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: NetFrameSplitor.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using System;


public class DataBuff
{
    byte[] dataBuff;

    int writePos;
    int readPos;


    public int Length
    {
        get
        {
            return writePos - readPos;
        }
    }


    public DataBuff(int capacity)
    {
        writePos = 0;
        dataBuff = new byte[capacity];
    }


    private int Left
    {
        get
        {
            return dataBuff.Length - writePos;
        }
    }


    private void ClearDirty()
    {
        if (readPos > 0)
        {
            for (int i = 0; i < readPos; i++)
            {
                dataBuff[i] = dataBuff[readPos];
            }
            writePos -= readPos;
        }
    }


    public void Write(byte[] data, int offset, int length)
    {
        if (data == null || data.Length == 0)
        {
            return;
        }

        if (length > data.Length)
        {
            throw new Exception("lenght out of range");
        }
        if (offset < 0 || offset >= data.Length)
        {
            throw new Exception("index out of range");
        }
        if (offset + length > data.Length)
        {
            throw new Exception("lenght with the offset is out of range");
        }


        if (this.Left < length)
        {
            this.ClearDirty();
        }

        if (this.Left < length)
        {
            throw new Exception("out of range");
        }

        Array.Copy(data, offset, dataBuff, writePos, length);
        writePos += length;
    }


    public int GetInt()
    {
        if (this.Length < 4)
            throw new Exception("read int error, need 4 byte");
        int length = BitConverter.ToInt32(dataBuff, readPos);
        return length;
    }


    public byte[] ReadByte(int skip, int readLenght)
    {
        if (readLenght < 0)
            throw new Exception("read lenght equal or less than 0");

        if(skip < 0)
            throw new Exception("skip equal or less than 0");

        if (this.Length < skip + readLenght)
            throw new Exception("not empty data");

        readPos += skip;
        byte[] ret = new byte[readLenght];
        Array.Copy(dataBuff, readPos, ret, 0, readLenght);
        readPos += readLenght;

        return ret;
    }
}


public class NetFrameSplitor
{

    int maxFrameLenght;
    int maxFrameNum;

    DataBuff buff;

	public NetFrameSplitor()
    {
        maxFrameLenght = 10*1024*1024; //每个包1M
        maxFrameNum = 4;
        buff = new DataBuff(maxFrameNum * maxFrameLenght);
    }


    public void Write(byte[] data, int offset, int length)
    {
        buff.Write(data, offset, length);
    }


    public byte[] Split()
    {
        if (this.buff.Length < 4)
        {
            return null;
        }

        int length = this.buff.GetInt();

        if (length <= 0)
            throw new Exception("frame body equal or less than zero");

        if (length > maxFrameLenght - 4)
        {
            throw new Exception("frame body out of max value, frame max len:1M");
        }

        if (this.buff.Length < length + 4)
        {
            return null;
        }

        byte[] frame = this.buff.ReadByte(4, length);
        return frame;
    }

}
