using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireControlSystem
{
    /// <summary>
    /// �����̴� ǥ���� ���߱� ���� ������ ���
    /// </summary>
    /// <param name="originPosition">������ġ</param>
    /// <param name="target">Ÿ�� ������ٵ� </param>
    /// <param name="bulletSpeed">�Ѿ� �ӵ�</param>
    /// <param name="adjustAccurateLoop">��Ȯ�� ���� �����Ҽ��� ��Ȯ��</param>
    /// <returns></returns>
    /// 

    

    public static float GetTimeToTarget(Vector3 originPos, Vector3 targetPos, float speed)
    {
        return Vector3.Distance(targetPos, originPos) / speed;
    }

    internal static Vector3 CalcFireDirection(Vector3 originPosition, Rigidbody target, float bulletSpeed, int accLoop, Vector3 velDelta)
    {
        float timeToTarget = 0f;
        Vector3 resultPredictedPos = target.position;
        for (int i = 0; i < accLoop; i++)
        {
            //Ÿ�� ��ġ���� ���� �ð�
            timeToTarget = GetTimeToTarget(originPosition, resultPredictedPos, bulletSpeed);
            //������ ��ġ
            // Predicted position based on kinematic equation: s = ut + 0.5at^2
            resultPredictedPos = target.position + (target.velocity * timeToTarget) + ( .5f * velDelta * timeToTarget * timeToTarget);
        }
        Vector3 ToTarget = (-originPosition + resultPredictedPos).normalized;
        return ToTarget;

    }



}
