using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetUi : MonoBehaviour
{
    private float _minx;
    private float _maxx;
    private float _miny;
    private float _maxy;
    private float _ratio;
    private VehicleCombat _vehicleCombat;
    private Transform _virtualMinimapPlayerAxis;

    public void Init(VehicleCombat vehicleCombat, Transform virtualMinimapPlayerAxis ,float minx, float maxx, float miny, float maxy
        , float iconSize, float ratio)
    {
        _vehicleCombat = vehicleCombat;
        _virtualMinimapPlayerAxis = virtualMinimapPlayerAxis;
        RectTransform rect = GetComponent<RectTransform>();
        //크기조절
        rect.sizeDelta = new Vector2(iconSize, iconSize);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        _minx = minx;
        _maxx = maxx;
        _miny = miny;
        _maxy = maxy;
        _ratio = ratio;
    }

    private void Update()
    {
        if(_vehicleCombat == null) 
        {
            Destroy(gameObject);
            return;
        }
        //위치 로컬로 변환
        Vector3 resultPos = _virtualMinimapPlayerAxis.InverseTransformPoint(_vehicleCombat.transform.position);
        resultPos = new Vector3(resultPos.x, resultPos.z, 0f);
        resultPos *= _ratio;
        resultPos = new Vector3(Mathf.Clamp(resultPos.x, _minx, _maxx), Mathf.Clamp(resultPos.y, _miny, _maxy), 0f);
        transform.localPosition = resultPos;
        //방향 로컬로 변환
        //보고있는 방향을 기준으로 월드 로테이션을 받아 y 축을 스프라이트의 z축에 넣고싶음
        //문제발생 뒤집어져있는 녀석들 발견
        Quaternion _currentItemLookRot = Quaternion.LookRotation(-_vehicleCombat.transform.forward);
        Quaternion resultRot = _currentItemLookRot * Quaternion.Inverse(_virtualMinimapPlayerAxis.rotation);
        transform.rotation = Quaternion.Euler(180f, 0f, (resultRot.eulerAngles.y));
    }
}
