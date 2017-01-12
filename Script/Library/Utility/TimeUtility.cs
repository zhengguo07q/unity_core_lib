using System;

public class TimeUtility
{
    public static string FormatSecTime(long Tr,int type=1)
    {
        string formatTime="";
        long day = Tr / (24 * 1000 * 60 * 60);
        long hour = ((Tr - day * (24 * 1000 * 60 * 60)) / (1000 * 60 * 60));
        long min = ((Tr - day * (24 * 1000 * 60 * 60) - hour * (1000 * 60 * 60)) / (1000 * 60));
        long sec = ((Tr - day * (24 * 1000 * 60 * 60) - hour * (1000 * 60 * 60) - min * (1000 * 60))) / 1000;
        if (type == 1)
            formatTime = sec.ToString();
        else if (type == 2)
            formatTime = min + ":" + sec;
        else if (type == 3)
            formatTime = (hour > 9 ? hour.ToString() : "0" + hour) + ":" + (min > 9 ? min.ToString() : "0" + min) + ":" + (sec > 9 ? sec.ToString() : "0" + sec);
        return formatTime;
    }


    public static string FormatDate(long millisecondDate)
    {
        DateTime dt_1970 = new DateTime(1970, 1, 1);
        long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
        long time_tricks = tricks_1970 + millisecondDate * 10000;//日志日期刻度
        DateTime dt = new DateTime(time_tricks);//转化为DateTime

        return (dt.Month > 9 ? dt.Month.ToString() : "0" + dt.Month) + (dt.Day > 9 ? dt.Day.ToString() : "0" + dt.Day);
    }


    /// 根据秒数返回字符串(hh:mm:ss)（可超过24小时）,北京时间跟UTC差8个小时
	public static string GetStringToChinese(int second)
    {
        return string.Format("{0:00}:{1:00}:{2:00}", second / 3600 % 24 + 8, second % 3600 / 60, second % 3600 % 60);
    }
}
