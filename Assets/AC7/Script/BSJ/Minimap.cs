using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Minimap : MonoBehaviour
{
    [SerializeField] GameObject targetPrefab;
    [SerializeField] float _ratio;
    [SerializeField] Transform _virtualMinmapPlayerAxis;
    [SerializeField] Transform _leftTopBoundary;
    [SerializeField] Transform _rightBottomBoundary;
    [SerializeField] float _iconSIze = 30f;
    float _minx;
    float _maxx;
    float _miny;
    float _maxy;

    private void Start()
    {
        Init();

        kjh.GameManager.Instance.OnTargetAdded += InitTargetUi;
    }

    private void Init()
    {
        _minx = _leftTopBoundary.localPosition.x;
        _maxx = _rightBottomBoundary.localPosition.x;
        _miny = _rightBottomBoundary.localPosition.y;
        _maxy = _leftTopBoundary.localPosition.y;
        foreach (VehicleCombat combat in kjh.GameManager.Instance.activeTargetList)
        {
            InitTargetUi(combat);
        }
    }

    private void InitTargetUi(VehicleCombat combat)
    {
        //角青矫 葛电 利 酒捞能 积己
        GameObject uiItem = Instantiate(targetPrefab, transform);
        uiItem.GetComponent<TargetUi>().Init(combat, _virtualMinmapPlayerAxis,
            _minx, _maxx, _miny, _maxy,
            _iconSIze, _ratio);
    }
}
