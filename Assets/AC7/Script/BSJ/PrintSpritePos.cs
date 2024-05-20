using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  ����׿� ���� ��������Ʈ�� ����, ���� ������ Ȯ��
///  bool ���� �ٲ�� OnValidate �����Ϳ����� ȣ���
/// </summary>
public class PrintSpritePos : MonoBehaviour
{
    [SerializeField] bool _update = false;
    [SerializeField] Vector3 _result;
    [SerializeField] Vector3 _localResult;
    private void OnValidate()
    {
        _result = transform.position;
        _localResult = transform.localPosition;
    }
}
