using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapTargetUi : MonoBehaviour
{
    
    private float _minx;
    private float _maxx;
    private float _miny;
    private float _maxy;
    private float _ratio;
    private VehicleCombat _vehicleCombat;
    private Transform _targetTransform;
    private Transform _virtualMinimapPlayerAxis;

    public void Init(Transform transform, Transform virtualMinimapPlayerAxis 
        , float minx, float maxx, float miny, float maxy
        , float iconSize, float ratio)
    {
        _vehicleCombat = transform.GetComponent<VehicleCombat>();
        _targetTransform = _vehicleCombat?.transform ?? transform;
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

        if(_vehicleCombat != null)
        {
            _vehicleCombat.onDead.AddListener(RemoveTargetUi);
        }
        else
        {
            transform.GetComponent<Guided>().OnRemove += RemoveTargetUi;
        }
    }

    private void Update()
    {
        if(_targetTransform == null)
        {
            return;
        }

        //위치 로컬로 변환
        Vector3 resultPos = _virtualMinimapPlayerAxis.InverseTransformPoint(_targetTransform.position);
        resultPos = new Vector3(resultPos.x, resultPos.z, 0f);
        resultPos *= _ratio;
        resultPos = new Vector3(Mathf.Clamp(resultPos.x, _minx, _maxx), Mathf.Clamp(resultPos.y, _miny, _maxy), 0f);
        transform.localPosition = resultPos;
        //방향 로컬로 변환
        Quaternion _currentItemLookRot = Quaternion.LookRotation(-_targetTransform.forward);
        Quaternion resultRot = _currentItemLookRot * Quaternion.Inverse(_virtualMinimapPlayerAxis.rotation);
        transform.rotation = Quaternion.Euler(180f, 0f, (resultRot.eulerAngles.y));
    }

    private void RemoveTargetUi()
    {
        Destroy(gameObject);
    }
}
