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
    Vector3 _gap;
    Vector3 _offset;

    Guided _missileTarget;

    public void Init(Transform transform, Transform virtualMinimapPlayerAxis 
        , float minx, float maxx, float miny, float maxy
        , float iconSize, float ratio, Vector3 offset)
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
            _missileTarget = transform.GetComponent<Guided>();
            _missileTarget.OnRemove += RemoveTargetUi;
        }
        _gap = new Vector3((_maxx - _minx)/2f, (_maxy - _miny)/2f, 0f);
        _offset = offset;
    }

    private void Update()
    {
        if(_targetTransform == null)
        {
            return;
        }

        //위치 로컬로 변환
        //Vector3 resultPos = _virtualMinimapPlayerAxis.InverseTransformPoint(_targetTransform.position);
        //resultPos = new Vector3(resultPos.x, resultPos.z, 0f);
        //resultPos *= _ratio;
        //resultPos = new Vector3(Mathf.Clamp(resultPos.x, _minx, _maxx), Mathf.Clamp(resultPos.y, _miny, _maxy), 0f);
        //transform.localPosition = resultPos;
        ///////////////////////////////////////////////////////////////////////////////////////////////
        //Transform playerTrf = kjh.GameManager.Instance.player.transform;
        //Vector3 playerPos = playerTrf.position;
        //Vector3 playerToTarget = -playerPos + _targetTransform.position;

        //Vector3 result = Quaternion.Inverse(playerTrf.rotation) * playerToTarget;
        //result.y = 0f;
        //result = result.normalized;
        //float dist = playerToTarget.magnitude;

        //result *= dist;
        Vector3 resultPos = _virtualMinimapPlayerAxis.InverseTransformPoint(_targetTransform.position);
        resultPos = new Vector3(resultPos.x, resultPos.z, 0f);
        resultPos *= _ratio;

        Vector3 dir = resultPos.normalized;
        bool xin = resultPos.y < _miny || resultPos.y > _maxy;
        _gap.y = (resultPos.y > 0f) ? _maxy : Mathf.Abs(_miny);
        if (Mathf.Abs(resultPos.x) > _gap.x || Mathf.Abs(resultPos.y) > _gap.y) 
        {
            Vector3 dist3D = new Vector3(Mathf.Abs(_gap.x / dir.x), Mathf.Abs(_gap.y / dir.y), 0f);
            float dist = Mathf.Min(dist3D.x, dist3D.y);
            transform.localPosition = dir * dist;
        }
        else
        {

            transform.localPosition = dir * resultPos.magnitude;
        }



        //방향 로컬로 변환
        Quaternion _currentItemLookRot = Quaternion.LookRotation(-_targetTransform.forward);
        Quaternion resultRot = _currentItemLookRot * Quaternion.Inverse(_virtualMinimapPlayerAxis.rotation);
        transform.rotation = Quaternion.Euler(180f, 0f, (resultRot.eulerAngles.y));
    }

    private void RemoveTargetUi()
    {
        if (_vehicleCombat != null)
        {
            _vehicleCombat.onDead.RemoveListener(RemoveTargetUi);
        }
        else
        {
            if(_missileTarget != null)
                _missileTarget.OnRemove -= RemoveTargetUi;
        }
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
