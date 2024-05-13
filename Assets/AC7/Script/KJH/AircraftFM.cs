using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftFM : MonoBehaviour
{
    public GameObject controlAircraft;
    AircraftData aircraftData;
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Awake()
    {
        aircraftData = controlAircraft.GetComponent<AircraftData>();
        rigidbody = this.gameObject.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rigidbody.velocity = this.transform.forward * 200;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 velocity = rigidbody.velocity;
        float velocitySpeed = velocity.magnitude;
        //Debug.Log(velocity);

        //���� �߷� ����
        rigidbody.AddForce(this.transform.forward * aircraftData.EnginePower(velocitySpeed), ForceMode.Acceleration);
        //��ġ ��ũ ����
        rigidbody.AddTorque(this.transform.right * -aircraftData.PitchTorque(velocitySpeed), ForceMode.Acceleration);
        //�� ��ũ ����
        rigidbody.AddTorque(this.transform.forward * -aircraftData.RollTorque(velocitySpeed), ForceMode.Acceleration);
        //�� ��ũ ����
        rigidbody.AddTorque(this.transform.up * aircraftData.YawTorque(velocitySpeed), ForceMode.Acceleration);
        
        Vector3 localVelocityDir = this.transform.InverseTransformDirection(rigidbody.velocity).normalized;
        //���� ������
        float aoa = -Mathf.Asin(localVelocityDir.y) * Mathf.Rad2Deg;
        //���� ������
        float aoaSide = -Mathf.Asin(localVelocityDir.x) * Mathf.Rad2Deg;

        //�ӵ� ������ ���� �������� ��� ����
        rigidbody.AddForce(Vector3.Cross(velocity, this.transform.right).normalized * aircraftData.GetLiftPower(aoa, velocitySpeed), ForceMode.Acceleration);
        rigidbody.AddForce(this.transform.right.normalized * aircraftData.GetLiftPower(aoaSide, velocitySpeed), ForceMode.Acceleration);
        //�ӵ� ������ �ݴ� �������� �����׷� �� �����׷� ����
        rigidbody.AddForce(-velocity.normalized * (aircraftData.GetInducedDrag(aoa, velocitySpeed) + aircraftData.GetParasiteDrag(aoa, velocitySpeed)), ForceMode.Acceleration);

        //�׷� ����
        rigidbody.drag = Atmosphere.Drag(this.transform.position.y, aircraftData.GetDC(), velocitySpeed);
        //Debug.Log(velocitySpeed);
    }
}
