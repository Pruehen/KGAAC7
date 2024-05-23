using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("모터 지속 시간")]
    [SerializeField] float boostStartDelay;//부스트 시작 딜레이 시간
    [SerializeField] float boostTime;//부스트 연소 시간
    [SerializeField] float sustainTime;//서스테인 연소 시간

    [Header("모터 추력")]
    [SerializeField] float boostPower;//부스트 연소 가속력
    [SerializeField] float sustainPower;//서스테인 연소 가속력

    [Header("FM")]
    [SerializeField] float liftPower;//양력
    float cD;//항력계수 (양력과 비례함)

    [SerializeField] float maxLiftTime;//최대 작동 시간. 초과시 자폭
    [SerializeField] float safeTime;//안전 시간. 이 시간이 지난 후 콜리더 활성화

    [SerializeField] GameObject explosionEffect;
    //[SerializeField] GameObject trail;
    [SerializeField] ParticleSystem smoke;
    [SerializeField] ParticleSystem motor;

    [Header("비행 데이터")]
    [SerializeField] float speed;
    [SerializeField] float sideForcef; 

    float lifeTime = 0;
    Rigidbody rigidbody;
    SphereCollider sphereCollider;
    bool fuseOn;
    public bool isCombustion { get; private set; }//현재 연소 중인지 판별
    Vector3 sideForce;
    /// <summary>
    /// 현재 로켓이 받고 있는 측가속도(Vector3)를 반환함.
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
        Vector3 velocity = rigidbody.velocity;
        float velocitySpeed = velocity.magnitude;
        speed = velocitySpeed;
        sideForcef = sideForce.magnitude;

        lifeTime += Time.fixedDeltaTime;
        rigidbody.drag = Atmosphere.Drag(this.transform.position.y, cD, velocitySpeed);

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

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("충돌");
        VehicleCombat fightable;
        if(collision.transform.TryGetComponent<VehicleCombat>(out fightable))
        {
            fightable.TakeDamage(GetComponent<WeaponData>().Dmg());
            EffectManager.Instance.EffectGenerate(explosionEffect, collision.transform.position);
            this.DestroyRocket();
            if (fightable.isPlayer)
            {
                kjh.GameManager.Instance.cameraShake.MissileHitShake();
            }
        }
        else
        {
            EffectManager.Instance.EffectGenerate(explosionEffect, collision.contacts[0].point);
            this.DestroyRocket();
        }

    }

    void DestroyRocket()
    {
        Guided guided;
        if(TryGetComponent<Guided>(out guided))
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
