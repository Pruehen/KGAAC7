using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

//해당 항공기의 FM 데이터
public class AircraftData : MonoBehaviour
{
    [SerializeField] float enginePower;//엔진 추력(가속도)
    [SerializeField] float pitchTorque;
    [SerializeField] float rollTorque;
    [SerializeField] float yawTorque;

    [SerializeField] AnimationCurve enginePowerCurve; //엔진 추력 커브
    [SerializeField] AnimationCurve torqueCurve; //토크 커브
    [SerializeField] AnimationCurve clCurve;//받음각에 따른 양력 계수 커브
    [SerializeField] AnimationCurve cdCurve;//받음각에 따른 유해항력 계수 커브
    [SerializeField][Range(0, 1)] float e;//스팬효율계수 (0~1까지의 값을 가짐)
    [SerializeField] float liftPower;//비례익면적.

    [SerializeField] float dragCoefficient;//항력 계수

    AircraftControl aircraftControl;
    private void Awake()
    {
        aircraftControl = this.gameObject.GetComponent<AircraftControl>();
    }
    /// <summary>
    /// 현재 속도에 따른 엔진 추력을 반환하는 메서드
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public float EnginePower(float speed, float altitude)
    {
        float axis = aircraftControl.throttle;
        if (axis >= 0)
        {
            return enginePower * enginePowerCurve.Evaluate(speed) * axis * Atmosphere.AtmosphericPressure(altitude * 0.5f);
        }
        else
        {
            return 0;
        }
    }
    /// <summary>
    /// 현재 속도에 따른 피치 축 토크를 반환하는 메서드
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public float PitchTorque(float speed)
    {
        float pitch;
        if(aircraftControl.pitch > 0)
        {
            pitch = aircraftControl.pitch;
        }
        else
        {
            pitch = aircraftControl.pitch * 0.5f;
        }
        return pitchTorque * torqueCurve.Evaluate(speed) * pitch;
    }
    /// <summary>
    /// 항공기가 저속일 때 스톨 토크를 반환하는 메서드
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public float StallTorque(float speed)
    {
        if (speed > 300)
            return 0;

        float value = torqueCurve.Evaluate(speed);
        if (value > 0.45f)
        {
            return 0;
        }
        else
        {
            return (0.45f - value) * 20;
        }    
    }
    /// <summary>
    /// 현재 속도에 따른 롤 축 토크를 반환하는 메서드
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public float RollTorque(float speed)
    {
        return rollTorque * torqueCurve.Evaluate(speed) * aircraftControl.roll;
    }
    /// <summary>
    /// 현재 속도에 따른 요 축 토크를 반환하는 메서드
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public float YawTorque(float speed)
    {
        return yawTorque * torqueCurve.Evaluate(speed) * aircraftControl.yaw;
    }
    /// <summary>
    /// 항력 계수를 반환하는 메서드. 에어브레이크 기능 추가됨
    /// </summary>
    /// <returns></returns>
    public float GetDC()
    {
        float axis = aircraftControl.throttle;
        if(axis >= 0)
        {
            return dragCoefficient;
        }
        else
        {
            return (1 + (-axis * 2)) * dragCoefficient;
        }
    }
    /// <summary>
    /// 받음각에 따른 양력을 반환하는 메서드
    /// </summary>
    /// <param name="aoa"></param>
    /// <returns></returns>
    public float GetLiftPower(float aoa, float speed)
    {        
        return clCurve.Evaluate(aoa) * liftPower * e * Atmosphere.AtmosphericPressure(transform.position.y) * speed * speed * 0.0001f;
    }
    /// <summary>
    /// 받음각과 속도에 따른 유도 항력을 반환하는 메서드
    /// </summary>
    /// <param name="aoa"></param>
    /// <returns></returns>
    public float GetInducedDrag(float aoa, float speed)
    {
        return Mathf.Abs(clCurve.Evaluate(aoa) * liftPower * (1 - e) * Atmosphere.AtmosphericPressure(transform.position.y) * speed * speed * 0.00005f);
    }
    /// <summary>
    /// 받음각과 속도에 따른 유해 항력을 반환하는 메서드
    /// </summary>
    /// <returns></returns>
    public float GetParasiteDrag(float aoa, float speed)
    {
        return cdCurve.Evaluate(aoa) * liftPower * Atmosphere.AtmosphericPressure(transform.position.y) * speed * speed * 0.00005f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
