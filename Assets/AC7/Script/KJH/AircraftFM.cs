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
        //�ڼ��� ���� �߰� ���� �׷� ���
        float dragGain_aoa = 1 + new Vector3(localVelocityDir.x, localVelocityDir.y, 0).magnitude * 5;

        //�ӵ� ������ ���� �������� ��� ����
        rigidbody.AddForce(Vector3.Cross(velocity, this.transform.right).normalized * aircraftData.GetLiftPower(aoa, velocitySpeed), ForceMode.Acceleration);
        //�ӵ� ������ �ݴ� �������� �����׷� ����
        rigidbody.AddForce(-velocity.normalized * aircraftData.GetInducedDrag(aoa, velocitySpeed), ForceMode.Acceleration);

        //�׷� ����
        rigidbody.drag = Atmosphere.Drag(this.transform.position.y, aircraftData.GetDC() * dragGain_aoa, velocitySpeed);
        Debug.Log(velocitySpeed);
    }
}
