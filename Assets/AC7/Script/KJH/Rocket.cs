using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : NetworkBehaviour
{
    [Header("���� ���� �ð�")]
    [SerializeField] float boostStartDelay;//�ν�Ʈ ���� ������ �ð�
    [SerializeField] float boostTime;//�ν�Ʈ ���� �ð�
    [SerializeField] float sustainTime;//�������� ���� �ð�

    [Header("���� �߷�")]
    [SerializeField] float boostPower;//�ν�Ʈ ���� ���ӷ�
    [SerializeField] float sustainPower;//�������� ���� ���ӷ�

    [Header("FM")]
    [SerializeField] float liftPower;//���
    float cD;//�׷°�� (��°� �����)

    [SerializeField] float maxLiftTime;//�ִ� �۵� �ð�. �ʰ��� ����
    [SerializeField] float safeTime;//���� �ð�. �� �ð��� ���� �� �ݸ��� Ȱ��ȭ

    [SerializeField] GameObject explosionEffect;
    [SerializeField] GameObject _waterHitVfx;
    //[SerializeField] GameObject trail;
    [SerializeField] ParticleSystem smoke;
    [SerializeField] ParticleSystem motor;

    [Header("���� ������")]
    [SerializeField] float speed;
    [SerializeField] float sideForcef; 

    float lifeTime = 0;
    Rigidbody rigidbody;
    SphereCollider sphereCollider;
    bool fuseOn;
    public bool isCombustion { get; private set; }//���� ���� ������ �Ǻ�
    Vector3 sideForce;
    /// <summary>
    /// ���� ������ �ް� �ִ� �����ӵ�(Vector3)�� ��ȯ��.
    /// </summary>
    /// <returns></returns>
    public Vector3 SideForce() { return sideForce; }

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rigidbody = GetComponent<Rigidbody>();
        isCombustion = false;
        cD = liftPower * 0.015f;

        sphereCollider.enabled = false;
        fuseOn = false;

        //trail.SetActive(false);
        smoke.gameObject.SetActive(false);
        motor.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isServer)
        {
            lifeTime += Time.fixedDeltaTime;
            if (lifeTime > maxLiftTime)
            {
                this.DestroyRocket();
            }
            if (lifeTime > boostStartDelay && !isCombustion)
            {
                isCombustion = true;
                //trail.SetActive(true);
                smoke.gameObject.SetActive(true);
                motor.gameObject.SetActive(true);
                smoke.Play();
                motor.Play();
            }
            if (isCombustion)
            {
                if (lifeTime < boostTime + boostStartDelay)
                {
                }
                else if (lifeTime < boostTime + sustainTime + boostStartDelay)
                {
                }
                else
                {
                    isCombustion = false;
                    //smoke.Stop();
                    motor.Stop();
                }
            }
            return;
        }
        Vector3 velocity = rigidbody.velocity;
        float velocitySpeed = velocity.magnitude;
        speed = velocitySpeed;
        sideForcef = sideForce.magnitude;

        rigidbody.drag = Atmosphere.Drag(this.transform.position.y, cD, velocitySpeed);

        lifeTime += Time.fixedDeltaTime;
        if (lifeTime > maxLiftTime)
        {
            this.DestroyRocket();
        }
        if (lifeTime > safeTime && !fuseOn)
        {
            sphereCollider.enabled = true;
            fuseOn = true;
        }
        if (lifeTime > boostStartDelay && !isCombustion)
        {
            isCombustion = true;
            //trail.SetActive(true);
            smoke.gameObject.SetActive(true);
            motor.gameObject.SetActive(true);
            smoke.Play();
            motor.Play();
        }

        if (isCombustion)
        {
            if (lifeTime < boostTime + boostStartDelay)
            {
                Combustion(boostPower);
            }
            else if (lifeTime < boostTime + sustainTime + boostStartDelay)
            {
                Combustion(sustainPower);
            }
            else
            {
                isCombustion = false;
                //smoke.Stop();
                motor.Stop();
            }
        }

        sideForce = this.transform.forward * velocitySpeed - velocity;
        rigidbody.AddForce(sideForce, ForceMode.Acceleration);

        //Debug.Log(velocitySpeed);
    }

    void Combustion(float power)
    {
        rigidbody.AddForce(this.transform.forward * power, ForceMode.Acceleration);
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("�浹");
        VehicleCombat fightable;
        if(collision.collider.TryGetComponent<VehicleCombat>(out fightable))
        {
            fightable.TakeDamage(GetComponent<WeaponData>().Dmg());
            Vector3 contact = collision.GetContact(0).point;
            RpcOnPlayerHit(fightable.netId, contact);
        }
        else
        {
            Vector3 contact = collision.GetContact(0).point;

            if (collision.transform.CompareTag("Water"))
            {
                RpcOnWaterHit(contact);
            }
            RpcOnHit(contact);
        }

    }
    [ClientRpc]
    private void RpcOnPlayerHit(uint targetNetId, Vector3 contact)
    {
        EffectManager.Instance.EffectGenerate(explosionEffect, contact);
        if (netId == kjh.GameManager.Instance.player.vehicleCombat.netId)
        {
            kjh.GameManager.Instance.cameraShake.MissileHitShake();
        }
        Excute_DestroyRocket();
    }

    [ClientRpc]
    private void RpcOnWaterHit(Vector3 position)
    {
        OnWaterHit(position);
    }
    private void OnWaterHit(Vector3 position)
    {
        EffectManager.Instance.EffectGenerate(_waterHitVfx, position);
    }
    [ClientRpc]
    private void RpcOnHit(Vector3 position)
    {
        OnHit(position);
    }
    private void OnHit(Vector3 position)
    {
        EffectManager.Instance.EffectGenerate(explosionEffect, position);
        Excute_DestroyRocket();
    }

    void DestroyRocket()
    {
        if(isServer)
        {
            Excute_DestroyRocket();
            RpcDestroyRocket();
        }
    }

    [ClientRpc]
    private void RpcDestroyRocket()
    {
        Excute_DestroyRocket();
    }

    private void Excute_DestroyRocket()
    {

        Guided guided;
        if (TryGetComponent<Guided>(out guided))
        {
            guided.RemoveTarget();
        }

        Destroy(motor.gameObject);

        smoke.transform.SetParent(null);
        //trail.transform.SetParent(null);

        smoke.Stop();
        //Destroy(trail.gameObject, 10);
        Destroy(smoke.gameObject, 10);

        Destroy(this.gameObject);
    }

}
