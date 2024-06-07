using UnityEngine;
using Mirror;

//������� �װ����� FM �����͸� �޾Ƽ� ���� ���� ������ ����
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

    [Header("���� ����")]
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
        //���� ������
        float aoa = -Mathf.Asin(localVelocityDir.y) * Mathf.Rad2Deg;
        AoA = (int)aoa;
        //���� ������
        float aoaSide = -Mathf.Asin(localVelocityDir.x) * Mathf.Rad2Deg;
        //Debug.Log(velocity);

        _enginePower = aircraftData.EnginePower(velocitySpeed, this.transform.position.y);
        _pitchTorque = -aircraftData.PitchTorque(velocitySpeed, aoa) * _torqueGain;
        _rollTorque = -aircraftData.RollTorque(velocitySpeed, aoa) * _torqueGain;
        _yawTorque = aircraftData.YawTorque(velocitySpeed, aoa) * _torqueGain;

        //���� �߷� ����
        rigidbody.AddForce(this.transform.forward * _enginePower, ForceMode.Acceleration);
        //��ġ ��ũ ����
        rigidbody.AddTorque(this.transform.right * _pitchTorque, ForceMode.Acceleration);
        //�� ��ũ ����
        rigidbody.AddTorque(this.transform.forward * _rollTorque, ForceMode.Acceleration);
        //�� ��ũ ����
        rigidbody.AddTorque(this.transform.up * _yawTorque, ForceMode.Acceleration);
        //���� ��ũ ����
        Vector3 stallAxis = Vector3.Cross(this.transform.forward, new Vector3(0, -1, 0)) * _torqueGain;
        rigidbody.AddTorque(stallAxis * aircraftData.StallTorque(velocitySpeed), ForceMode.Acceleration);


        float liftPower = aircraftData.GetLiftPower(aoa, velocitySpeed);
        G_Force = liftPower * 0.1f;
        //�ӵ� ������ ���� �������� ��� ����
        rigidbody.AddForce(Vector3.Cross(velocity, this.transform.right).normalized * liftPower, ForceMode.Acceleration);
        //������ ��� ����
        rigidbody.AddForce(this.transform.right * velocitySpeed * velocitySpeed * aoaSide * 0.0001f, ForceMode.Acceleration);
        //�ӵ� ������ �ݴ� �������� �����׷� �� �����׷� ����
        rigidbody.AddForce(-velocity.normalized * (aircraftData.GetInducedDrag(aoa, velocitySpeed) + aircraftData.GetParasiteDrag(aoa, velocitySpeed)), ForceMode.Acceleration);

        //�׷� ����
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
