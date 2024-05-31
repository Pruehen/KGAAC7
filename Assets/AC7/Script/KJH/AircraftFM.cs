using UnityEngine;
using Mirror;
using Unity.VisualScripting;

//사용중인 항공기의 FM 데이터를 받아서 실제 비행 물리를 적용
public class AircraftFM : NetworkBehaviour
{
    AircraftSelecter aircraftSelecter;
    VaporEffect effect;
    [SerializeField] AircraftData aircraftData;
    Rigidbody rigidbody;

    [SerializeField] float _enginePower;
    [SerializeField] float _pitchTorque;
    [SerializeField] float _rollTorque;
    [SerializeField] float _yawTorque;

    [Header("동기 변수")]
    [SyncVar] [SerializeField] Vector3 _curVelocity;
    [SyncVar] [SerializeField] Vector3 _curPos;
    [SyncVar] [SerializeField] Quaternion _curRot;

    public void Init()
    {
        aircraftSelecter = GetComponent<AircraftSelecter>();
        effect = aircraftSelecter.controlAircraft.GetComponent<VaporEffect>();
        rigidbody = this.gameObject.GetComponent<Rigidbody>();
        rigidbody.velocity = this.transform.forward * 200;
    }

    void FixedUpdate()
    {
        FlightModelOnFixedUpdate();
        FlightDataSyncOnFixedUpdate();        
    }
    
    void FlightModelOnFixedUpdate()
    {
        aircraftData = aircraftSelecter.aircraftData;

        Vector3 velocity = rigidbody.velocity;
        float velocitySpeed = velocity.magnitude;
        //Debug.Log(velocity);

        _enginePower = aircraftData.EnginePower(velocitySpeed, this.transform.position.y);
        _pitchTorque = -aircraftData.PitchTorque(velocitySpeed);
        _rollTorque = -aircraftData.RollTorque(velocitySpeed);
        _yawTorque = aircraftData.YawTorque(velocitySpeed);

        //엔진 추력 적용
        rigidbody.AddForce(this.transform.forward * _enginePower, ForceMode.Acceleration);
        //피치 토크 적용
        rigidbody.AddTorque(this.transform.right * _pitchTorque, ForceMode.Acceleration);
        //롤 토크 적용
        rigidbody.AddTorque(this.transform.forward * _rollTorque, ForceMode.Acceleration);
        //요 토크 적용
        rigidbody.AddTorque(this.transform.up * _yawTorque, ForceMode.Acceleration);
        //스톨 토크 적용
        Vector3 stallAxis = Vector3.Cross(this.transform.forward, new Vector3(0, -1, 0));
        rigidbody.AddTorque(stallAxis * aircraftData.StallTorque(velocitySpeed), ForceMode.Acceleration);

        Vector3 localVelocityDir = this.transform.InverseTransformDirection(rigidbody.velocity).normalized;
        //정면 받음각
        float aoa = -Mathf.Asin(localVelocityDir.y) * Mathf.Rad2Deg;
        //측면 받음각
        float aoaSide = -Mathf.Asin(localVelocityDir.x) * Mathf.Rad2Deg;

        //속도 벡터의 수직 방향으로 양력 생성
        rigidbody.AddForce(Vector3.Cross(velocity, this.transform.right).normalized * aircraftData.GetLiftPower(aoa, velocitySpeed), ForceMode.Acceleration);
        rigidbody.AddForce(this.transform.right * velocitySpeed * velocitySpeed * aoaSide * 0.0001f, ForceMode.Acceleration);
        //속도 벡터의 반대 방향으로 유도항력 및 유해항력 생성
        rigidbody.AddForce(-velocity.normalized * (aircraftData.GetInducedDrag(aoa, velocitySpeed) + aircraftData.GetParasiteDrag(aoa, velocitySpeed)), ForceMode.Acceleration);

        //항력 적용
        rigidbody.drag = Atmosphere.Drag(this.transform.position.y, aircraftData.GetDC(), velocitySpeed);
        //Debug.Log(velocitySpeed);

        effect?.SetEffect(velocitySpeed, aoa);
    }

    void FlightDataSyncOnFixedUpdate()
    {
        if (this.isLocalPlayer)
        {
            _curVelocity = rigidbody.velocity;
            _curPos = this.transform.position;
            _curRot = this.transform.rotation;
        }
        else
        {
            rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, _curVelocity, Time.fixedDeltaTime);
            this.transform.position = Vector3.Lerp(this.transform.position, _curPos, Time.fixedDeltaTime);
            this.transform.rotation = _curRot;
        }        
    }
}
