using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere
{
    /// <summary>
    /// 리지드바디에 적용할 항력을 확인하는 메서드. 각각 현재 고도, 항력 계수, 속도값을 입력하면 된다.
    /// </summary>
    /// <param name="yPos"></param>
    /// <param name="dc"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static float Drag(float yPos, float cd, float speed)
    {        
        //Debug.Log(ap);

        //간단한 충격파 항력.
        float dragGain = 1 + (Mathf.Clamp(speed - 300, 0, 60)) / 30;

        return AtmosphericPressure(yPos) * cd * speed / 300 * dragGain;
    }
    /// <summary>
    /// 고도에 따른 대기압을 확인하는 메서드
    /// </summary>
    /// <param name="yPos"></param>
    /// <returns></returns>
    public static float AtmosphericPressure(float yPos)
    {
        //근사치. 5500미터 올라갈때마다 절반으로 감소함.
        return 1 / Mathf.Exp(-Mathf.Log(0.5f) / 5500 * yPos);
    }
}
