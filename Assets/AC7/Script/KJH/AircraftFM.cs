using UnityEngine;
using Mirror;
using Unity.VisualScripting;

//������� �װ����� FM �����͸� �޾Ƽ� ���� ���� ������ ����
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

    [Header("���� ����")]
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

        //���� �߷� ����
        rigidbody.AddForce(this.transform.forward * _enginePower, ForceMode.Acceleration);
        //��ġ ��ũ ����
        rigidbody.AddTorque(this.transform.right * _pitchTorque, ForceMode.Acceleration);
        //�� ��ũ ����
        rigidbody.AddTorque(this.transform.forward * _rollTorque, ForceMode.Acceleration);
        //�� ��ũ ����
        rigidbody.AddTorque(this.transform.up * _yawTorque, ForceMode.Acceleration);
        //���� ��ũ ����
        Vector3 stallAxis = Vector3.Cross(this.transform.forward, new Vector3(0, -1, 0));
        rigidbody.AddTorque(stallAxis * aircraftData.StallTorque(velocitySpeed), ForceMode.Acceleration);

        Vector3 localVelocityDir = this.transform.InverseTransformDirection(rigidbody.velocity).normalized;
        //���� ������
        float aoa = -Mathf.Asin(localVelocityDir.y) * Mathf.Rad2Deg;
        //���� ������
        float aoaSide = -Mathf.Asin(localVelocityDir.x) * Mathf.Rad2Deg;

        //�ӵ� ������ ���� �������� ��� ����
        rigidbody.AddForce(Vector3.Cross(velocity, this.transform.right).normalized * aircraftData.GetLiftPower(aoa, velocitySpeed), ForceMode.Acceleration);
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
            this.transform.position = Vector3.Lerp(this.transform.position, _curPos, Time.fixedDeltaTime);
            this.transform.rotation = _curRot;
        }        
    }
}
