using UnityEngine;
using System.Collections;

public static class DataManager
{
    /// <summary>
    /// 사운드 온/오프
    /// </summary>
    public static bool b_Sound = true;
    /// <summary>
    /// 진동 온/오프
    /// </summary>
    public static bool b_Vibration = true;
    /// <summary>
    /// 현재 점수
    /// </summary>
    public static int curScore = 0;

    public static int playCount = 0;
    public static float PlayTime;
    public static float Time;
}
