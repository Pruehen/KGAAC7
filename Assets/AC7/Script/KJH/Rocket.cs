using Mirror;
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
                smoke.Stop();
                motor.Stop();
            }
        }

        sideForce = (this.transform.forward * velocitySpeed - velocity) * liftPower * Atmosphere.AtmosphericPressure(this.transform.position.y) * Mathf.Min(lifeTime * 0.5f, 1);
        rigidbody.AddForce(sideForce, ForceMode.Acceleration);

        //Debug.Log(velocitySpeed);
    }

    void Combustion(float power)
    {
        rigidbody.AddForce(this.transform.forward * power, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("�浹");
        VehicleCombat fightable;
        if(collision.collider.TryGetComponent<VehicleCombat>(out fightable))
        {
            TargetTakeDamage(fightable, GetComponent<WeaponData>());
            //fightable.TakeDamage(GetComponent<WeaponData>().Dmg());
            Vector3 contact = collision.GetContact(0).point;
            EffectManager.Instance.EffectGenerate(explosionEffect, contact);
            this.DestroyRocket();
            if (fightable.isPlayer && fightable.isLocalPlayer)
            {
                kjh.GameManager.Instance.cameraShake.MissileHitShake();
            }
        }
        else
        {
            Vector3 contact = collision.GetContact(0).point;
            EffectManager.Instance.EffectGenerate(explosionEffect, contact);
            if (collision.transform.CompareTag("Water"))
            {
                EffectManager.Instance.EffectGenerate(_waterHitVfx, contact);
            }
            this.DestroyRocket();
        }
    }
    void TargetTakeDamage(VehicleCombat target, WeaponData weaponData)
    {
        target.TakeDamage(weaponData.Dmg());
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
