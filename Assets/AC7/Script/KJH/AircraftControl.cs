using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AircraftControl : MonoBehaviour
{
    [SerializeField] Transform _la; //좌측 에일러론
    [SerializeField] Transform _ra; //우측 에일러론
    [SerializeField] Transform _le; //좌측 엘리베이터
    [SerializeField] Transform _re; //우측 엘리베이터
    [SerializeField] Transform _lr; //좌측 러더
    [SerializeField] Transform _rr; //우측 러더

    Quaternion _laAxis;
    Quaternion _raAxis;
    Quaternion _leAxis;
    Quaternion _reAxis;
    Quaternion _lrAxis;
    Quaternion _rrAxis;

    [Header("조종면 가동 범위")]
    [SerializeField] float _a_Range;
    [SerializeField] float _e_Range;
    [SerializeField] float _r_Range;

    [Header("조종면 가동력")]
    [SerializeField] float _controlPower;

    [Header("엔진 추력 딜레이")]
    [SerializeField] float _engineDelay;

    float _pitchTarget;
    float _rollTarget;
    float _yawTarget;
    float _throttleTarget;

    public float pitch { get; private set; }//-1~1
    public float roll { get; private set; }//-1~1
    public float yaw { get; private set; }//-1~1
    public float throttle { get; private set; }//-1~1

    /// <summary>
    /// 항공기의 조종면에 축 회전값(float)을 입력하는 메서드. 각 축은 -1~1의 범위를 가져야 함
    /// </summary>
    /// <param name="pitch"></param>
    /// <param name="roll"></param>
    /// <param name="yaw"></param>
    public void SetAxisValue(float pitch, float roll, float yaw, float throttle)
    {
        this._pitchTarget = pitch;
        this._rollTarget = roll;
        this._yawTarget = yaw;
        this._throttleTarget = throttle;
    }

    private void Start()
    {
        _laAxis = _la.localRotation;
        _raAxis = _ra.localRotation;
        _leAxis = _le.localRotation;
        _reAxis = _re.localRotation;
        _lrAxis = _lr.localRotation;
        _rrAxis = _rr.localRotation;
    }
    // Update is called once per frame
    void Update()
    {
        SetAxisValue(PlayerInputCustom.Instance.pitchAxis, PlayerInputCustom.Instance.rollAxis, PlayerInputCustom.Instance.yawAxis, PlayerInputCustom.Instance.throttleAxis);//테스트 코드

        pitch = Mathf.Lerp(pitch, _pitchTarget, Time.deltaTime * _controlPower);
        roll = Mathf.Lerp(roll, _rollTarget, Time.deltaTime * _controlPower);
        yaw = Mathf.Lerp(yaw, _yawTarget, Time.deltaTime * _controlPower);
        throttle = Mathf.Lerp(throttle, _throttleTarget, Time.deltaTime * _engineDelay);

        _la.localRotation = _laAxis * Quaternion.Euler(roll * _a_Range, 0, 0);
        _ra.localRotation = _raAxis * Quaternion.Euler(-roll * _a_Range, 0, 0);

        _le.localRotation = _leAxis * Quaternion.Euler((roll * _e_Range + -pitch * _e_Range) * 0.5f, 0, 0);
        _re.localRotation = _reAxis * Quaternion.Euler((-roll * _e_Range + -pitch * _e_Range) * 0.5f, 0, 0);

        _lr.localRotation = _lrAxis * Quaternion.Euler(0, -yaw * _r_Range, 0);
        _rr.localRotation = _rrAxis * Quaternion.Euler(0, -yaw * _r_Range, 0);
    }

}
