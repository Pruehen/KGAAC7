using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  디버그용 현재 스프라이트의 로컬, 월드 포지션 확인
///  bool 값이 바뀌면 OnValidate 에디터에서만 호출됨
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
