using Cinemachine;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftFMNetwork : NetworkBehaviour, IFightable
{
    public GameObject controlAircraft;
    AircraftData _aircraftData;
    Rigidbody _rigidbody;
    WeaponSystem _weapon;
    Combat _combat = new Combat();
    // Start is called before the first frame update
    void Awake()
    {
        _aircraftData = controlAircraft.GetComponent<AircraftData>();
        _rigidbody = this.gameObject.GetComponent<Rigidbody>();
        _weapon = GetComponent<WeaponSystem>();
        _combat.Init(transform, 100f);
    }
    //ī�޶� �÷��̾� ������ ���̰� ����� ��
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (isLocalPlayer)
        {
            Camera.main.transform.parent.SetParent(transform);
            PlayerInputCustom.Instance.OnFirecus += _weapon.Fire;
        }
    }
    public override void OnStopLocalPlayer()
    {
        base.OnStopLocalPlayer();
        if (isLocalPlayer)
        {
            Camera.main.transform.parent.SetParent(null);
            PlayerInputCustom.Instance.OnFirecus -= _weapon.Fire;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 velocity = _rigidbody.velocity;
        float velocitySpeed = velocity.magnitude;
        //Debug.Log(velocity);

        //���� �߷� ����
        _rigidbody.AddForce(this.transform.forward * _aircraftData.EnginePower(velocitySpeed), ForceMode.Acceleration);
        //��ġ ��ũ ����
        _rigidbody.AddTorque(this.transform.right * -_aircraftData.PitchTorque(velocitySpeed), ForceMode.Acceleration);
        //�� ��ũ ����
        _rigidbody.AddTorque(this.transform.forward * -_aircraftData.RollTorque(velocitySpeed), ForceMode.Acceleration);
        //�� ��ũ ����
        _rigidbody.AddTorque(this.transform.up * _aircraftData.YawTorque(velocitySpeed), ForceMode.Acceleration);
        
        Vector3 localVelocityDir = this.transform.InverseTransformDirection(_rigidbody.velocity).normalized;
        //���� ������
        float aoa = -Mathf.Asin(localVelocityDir.y) * Mathf.Rad2Deg;
        //���� ������
        float aoaSide = -Mathf.Asin(localVelocityDir.x) * Mathf.Rad2Deg;

        //�ӵ� ������ ���� �������� ��� ����
        _rigidbody.AddForce(Vector3.Cross(velocity, this.transform.right).normalized * _aircraftData.GetLiftPower(aoa, velocitySpeed), ForceMode.Acceleration);
        _rigidbody.AddForce(this.transform.right.normalized * _aircraftData.GetLiftPower(aoaSide, velocitySpeed), ForceMode.Acceleration);
        //�ӵ� ������ �ݴ� �������� �����׷� �� �����׷� ����
        _rigidbody.AddForce(-velocity.normalized * (_aircraftData.GetInducedDrag(aoa, velocitySpeed) + _aircraftData.GetParasiteDrag(aoa, velocitySpeed)), ForceMode.Acceleration);

        //�׷� ����
        _rigidbody.drag = Atmosphere.Drag(this.transform.position.y, _aircraftData.GetDC(), velocitySpeed);
        Debug.Log(velocitySpeed);
    }

    public void Shoot()
    {
        if(isServer)
        {
            //_weapon.Fire();
        }
        else
        {
            _weapon.Fire();
        }
    }

    public void DealDamage(IFightable target, float damage)
    {
        target.TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        _combat.TakeDamage(damage);
    }
}
