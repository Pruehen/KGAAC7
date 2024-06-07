using UnityEngine;
using Mirror;

//사용중인 항공기의 FM 데이터를 받아서 실제 비행 물리를 적용
public class AircraftFM : NetworkBehaviour
{
    //AircraftSelecter aircraftSelecter;
    VaporEffect effect;
    [SerializeField] AircraftData aircraftData;
    Rigidbody rigidbody;

    [SerializeField] float _enginePower;
    [SerializeField] float _pitchTorque;
    [SerializeField] float _rollTorque;
    [SerializeField] float _yawTorque;
    [SerializeField] float _torqueGain;

    [Header("동기 변수")]
    [SyncVar] [SerializeField] Vector3 _curVelocity;
    [SyncVar] [SerializeField] Vector3 _curPos;
    [SyncVar] [SerializeField] Quaternion _curRot;

    public int Velocity { get; private set; }
    public float G_Force { get; private set; }
    public int AoA { get; private set; }

    bool isInit = false;
    public void Init(GameObject controlAircraft)
    {
        aircraftData = controlAircraft.GetComponent<AircraftData>();
        effect = controlAircraft.GetComponent<VaporEffect>();
        rigidbody = this.gameObject.GetComponent<Rigidbody>();
        InitSpeed();
        isInit = true;
    }

    public void InitSpeed()
    {
        rigidbody.velocity = this.transform.forward * 200;
        rigidbody.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (!isInit)
            return;

        FlightModelOnFixedUpdate();
        FlightDataSyncOnFixedUpdate();        
    }
    
    void FlightModelOnFixedUpdate()
    {
        //aircraftData = aircraftSelecter.aircraftData;

        Vector3 velocity = rigidbody.velocity;
        float velocitySpeed = velocity.magnitude;
        Velocity = (int)(velocitySpeed * 3.6f);

        Vector3 localVelocityDir = this.transform.InverseTransformDirection(rigidbody.velocity).normalized;
        //정면 받음각
        float aoa = -Mathf.Asin(localVelocityDir.y) * Mathf.Rad2Deg;
        AoA = (int)aoa;
        //측면 받음각
        float aoaSide = -Mathf.Asin(localVelocityDir.x) * Mathf.Rad2Deg;
        //Debug.Log(velocity);

        _enginePower = aircraftData.EnginePower(velocitySpeed, this.transform.position.y);
        _pitchTorque = -aircraftData.PitchTorque(velocitySpeed, aoa) * _torqueGain;
        _rollTorque = -aircraftData.RollTorque(velocitySpeed, aoa) * _torqueGain;
        _yawTorque = aircraftData.YawTorque(velocitySpeed, aoa) * _torqueGain;

        //엔진 추력 적용
        rigidbody.AddForce(this.transform.forward * _enginePower, ForceMode.Acceleration);
        //피치 토크 적용
        rigidbody.AddTorque(this.transform.right * _pitchTorque, ForceMode.Acceleration);
        //롤 토크 적용
        rigidbody.AddTorque(this.transform.forward * _rollTorque, ForceMode.Acceleration);
        //요 토크 적용
        rigidbody.AddTorque(this.transform.up * _yawTorque, ForceMode.Acceleration);
        //스톨 토크 적용
        Vector3 stallAxis = Vector3.Cross(this.transform.forward, new Vector3(0, -1, 0)) * _torqueGain;
        rigidbody.AddTorque(stallAxis * aircraftData.StallTorque(velocitySpeed), ForceMode.Acceleration);


        float liftPower = aircraftData.GetLiftPower(aoa, velocitySpeed);
        G_Force = liftPower * 0.1f;
        //속도 벡터의 수직 방향으로 양력 생성
        rigidbody.AddForce(Vector3.Cross(velocity, this.transform.right).normalized * liftPower, ForceMode.Acceleration);
        //측방향 양력 생성
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
            if(Vector3.Distance(this.transform.position, _curPos) > 1000)
            {
                this.transform.position = _curPos;
            }
            else
            {
                this.transform.position = Vector3.Lerp(this.transform.position, _curPos, Time.fixedDeltaTime);
            }
            this.transform.rotation = _curRot;
        }        
    }    
}
