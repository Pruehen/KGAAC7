using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//사용중인 항공기의 FM 데이터를 받아서 실제 비행 물리를 적용
public class AircraftFM : MonoBehaviour
{
    AircraftSelecter aircraftSelecter;
    [SerializeField] AircraftData aircraftData;
    Rigidbody rigidbody;    

    private void Awake()
    {
        aircraftSelecter = GetComponent<AircraftSelecter>();
        rigidbody = this.gameObject.GetComponent<Rigidbody>();
        rigidbody.velocity = this.transform.forward * 200;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        aircraftData = aircraftSelecter.aircraftData;

        Vector3 velocity = rigidbody.velocity;
        float velocitySpeed = velocity.magnitude;
        //Debug.Log(velocity);

        //엔진 추력 적용
        rigidbody.AddForce(this.transform.forward * aircraftData.EnginePower(velocitySpeed), ForceMode.Acceleration);
        //피치 토크 적용
        rigidbody.AddTorque(this.transform.right * -aircraftData.PitchTorque(velocitySpeed), ForceMode.Acceleration);
        //롤 토크 적용
        rigidbody.AddTorque(this.transform.forward * -aircraftData.RollTorque(velocitySpeed), ForceMode.Acceleration);
        //요 토크 적용
        rigidbody.AddTorque(this.transform.up * aircraftData.YawTorque(velocitySpeed), ForceMode.Acceleration);
        
        Vector3 localVelocityDir = this.transform.InverseTransformDirection(rigidbody.velocity).normalized;
        //정면 받음각
        float aoa = -Mathf.Asin(localVelocityDir.y) * Mathf.Rad2Deg;
        //측면 받음각
        float aoaSide = -Mathf.Asin(localVelocityDir.x) * Mathf.Rad2Deg;

        //속도 벡터의 수직 방향으로 양력 생성
        rigidbody.AddForce(Vector3.Cross(velocity, this.transform.right).normalized * aircraftData.GetLiftPower(aoa, velocitySpeed), ForceMode.Acceleration);
        rigidbody.AddForce(this.transform.right.normalized * aircraftData.GetLiftPower(aoaSide, velocitySpeed), ForceMode.Acceleration);
        //속도 벡터의 반대 방향으로 유도항력 및 유해항력 생성
        rigidbody.AddForce(-velocity.normalized * (aircraftData.GetInducedDrag(aoa, velocitySpeed) + aircraftData.GetParasiteDrag(aoa, velocitySpeed)), ForceMode.Acceleration);

        //항력 적용
        rigidbody.drag = Atmosphere.Drag(this.transform.position.y, aircraftData.GetDC(), velocitySpeed);
        //Debug.Log(velocitySpeed);
    }
}
