using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeCount
{
    public static DateTime t1,t2;
    public static double GetSubSeconds(DateTime startTimer, DateTime endTimer)
    {
        TimeSpan startSpan = new TimeSpan(startTimer.Ticks);

        TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);

        TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

       // ���ؼ�������������ķ��Ӻ�Сʱ�ȣ�������������֮��Ĳ
        //return subTimer.Seconds;

        //�������ʱ��
        return subTimer.TotalSeconds;
    }

}
