using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;

public class AntiAirGun : MonoBehaviour
{
    [SerializeField] private int _adjustAccurateLoop = 8;
    [SerializeField] private float _bulletSpeed = 360f;
    [SerializeField] private Rigidbody _target;
    [SerializeField] private GameObject _weapon;

    private void Start()
    {
        bsj.GameManager.Instance.OnPlayerSpawned += Init;
    }

    private void Init()
    {
        _target = bsj.GameManager.Instance.player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_target == null) return;
        Vector3 resultPredictedPos = _target.position;

        //Ÿ�� ��ġ���� ���� �ð�
        float timeToTarget = GetTimeToTarget(transform.position, resultPredictedPos, _bulletSpeed);
        //������ ��ġ
        resultPredictedPos = _target.position + _target.velocity * timeToTarget;

        for (int i = 0; i < _adjustAccurateLoop; i++)
        {
            //Ÿ�� ��ġ���� ���� �ð�
            timeToTarget = GetTimeToTarget(transform.position, resultPredictedPos, _bulletSpeed);
            //������ ��ġ
            resultPredictedPos = _target.position + _target.velocity * timeToTarget;
        }



        Vector3 ToTarget = (-transform.position + resultPredictedPos).normalized;
        GameObject item = Instantiate(_weapon, transform.position, Quaternion.identity);
        item.GetComponent<Rigidbody>().velocity = ToTarget * _bulletSpeed;
    }


    //Ÿ�ٱ����� �Ÿ������� Ÿ���� �ӵ��� �ð��� ����� ������
    private Vector3 CalcPredictedPos(Vector3 origin, Vector3 targetPos, Vector3 targetVel)
    {
        //Ÿ�� ��ġ���� ���� �ð�
        float timeToTarget = GetTimeToTarget(origin, targetPos, _bulletSpeed);
        //������ ��ġ
        Vector3 predictedPos = _target.position + targetVel * timeToTarget;
        return predictedPos;
    }

    private float GetTimeToTarget(Vector3 originPos, Vector3 targetPos, float speed)
    {
        return Vector3.Distance(targetPos, originPos) / speed;
    }
}
