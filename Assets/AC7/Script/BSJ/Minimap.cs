using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Minimap : MonoBehaviour
{
    [SerializeField] GameObject _targetPrefab;
    [SerializeField] GameObject _missilePrefab;
    [SerializeField] float _ratio;
    [SerializeField] Transform _virtualMinmapPlayerAxis;
    [SerializeField] Transform _leftTopBoundary;
    [SerializeField] Transform _rightBottomBoundary;
    [SerializeField] float _iconSIze = 30f;
    [SerializeField] Vector3 _offset = new Vector3(0f, 48f, 0f);
    float _minx;
    float _maxx;
    float _miny;
    float _maxy;

    private void Start()
    {
        Init();

        kjh.GameManager.Instance.OnTargetAdded += InitTargetUi;
        kjh.GameManager.Instance.OnMissileAdded += InitMissileUi;
    }

    private void Init()
    {
        _minx = _leftTopBoundary.localPosition.x;
        _maxx = _rightBottomBoundary.localPosition.x;
        _miny = _rightBottomBoundary.localPosition.y;
        _maxy = _leftTopBoundary.localPosition.y;
        foreach (VehicleCombat combat in kjh.GameManager.Instance.activeTargetList)
        {
            InitTargetUi(combat.transform);
        }
    }

    private void InitTargetUi(Transform targetTransform)
    {
        //타겟 아이콘 초기화
        GameObject uiItem = Instantiate(_targetPrefab, transform);

        VehicleCombat combat = targetTransform.GetComponent<VehicleCombat>();
        Color TargetColor = (combat.isPlayer) ? Color.blue : Color.red;

        uiItem.GetComponent<MinimapTargetUi>().Init(targetTransform, _virtualMinmapPlayerAxis,
            _minx, _maxx, _miny, _maxy,
            _iconSIze, _ratio, _offset, TargetColor);
    }
    private void InitMissileUi(Transform targetTransform)
    {
        //미사일 아이콘 초기화
        GameObject uiItem = Instantiate(_missilePrefab, transform);
        uiItem.GetComponent<MinimapTargetUi>().Init(targetTransform, _virtualMinmapPlayerAxis,
            _minx, _maxx, _miny, _maxy,
            _iconSIze, _ratio, _offset, Color.white);
    }
}
