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

    

    public static Vector3 CalcFireDirection(Vector3 originPosition, Rigidbody target, float bulletSpeed, int adjustAccurateLoop = 8)
    {
        Vector3 resultPredictedPos = target.position;

        for (int i = 0; i < adjustAccurateLoop; i++)
        {
            //Ÿ�� ��ġ���� ���� �ð�
            float timeToTarget = GetTimeToTarget(originPosition, resultPredictedPos, bulletSpeed);
            //������ ��ġ
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
