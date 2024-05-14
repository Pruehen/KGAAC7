using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireControlSystem
{
    /// <summary>
    /// 움직이는 표적을 맞추기 위한 방향을 계산
    /// </summary>
    /// <param name="originPosition">시작위치</param>
    /// <param name="target">타겟 리지드바디 </param>
    /// <param name="bulletSpeed">총알 속도</param>
    /// <param name="adjustAccurateLoop">정확도 루프 많이할수록 정확함</param>
    /// <returns></returns>
    /// 

    

    public static Vector3 CalcFireDirection(Vector3 originPosition, Rigidbody target, float bulletSpeed, int adjustAccurateLoop = 8)
    {
        Vector3 resultPredictedPos = target.position;

        for (int i = 0; i < adjustAccurateLoop; i++)
        {
            //타겟 위치까지 도착 시간
            float timeToTarget = GetTimeToTarget(originPosition, resultPredictedPos, bulletSpeed);
            //예측한 위치
            resultPredictedPos = target.position + target.velocity * timeToTarget;
        }
        Vector3 ToTarget = (-originPosition + resultPredictedPos).normalized;
        return ToTarget;
    }
    public static float GetTimeToTarget(Vector3 originPos, Vector3 targetPos, float speed)
    {
        return Vector3.Distance(targetPos, originPos) / speed;
    }
}
