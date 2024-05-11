using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("모터 지속 시간")]
    [SerializeField] float boostTime;//부스트 연소 시간
    [SerializeField] float sustainTime;//서스테인 연소 시간

    [Header("모터 추력")]
    [SerializeField] float boostPower;//부스트 연소 가속력
    [SerializeField] float sustainPower;//서스테인 연소 가속력

    [Header("FM")]
    [SerializeField] float liftPower;//양력
    float cD;//항력계수 (양력과 비례함)

    [SerializeField] float maxLiftTime;//최대 작동 시간. 초과시 자폭

    float lifeTime = 0;
    Rigidbody rigidbody;
    public bool isCombustion { get; private set; }//현재 연소 중인지 판별
    Vector3 sideForce;
    /// <summary>
    /// 현재 로켓이 받고 있는 측가속도(Vector3)를 반환함.
    /// </summary>
    /// <returns></returns>
    public Vector3 SideForce() { return sideForce; }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        isCombustion = true;
        cD = liftPower * 0.02f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 velocity = rigidbody.velocity;
        float velocitySpeed = velocity.magnitude;

        lifeTime += Time.fixedDeltaTime;
        rigidbody.drag = Atmosphere.Drag(this.transform.position.y, cD, velocitySpeed);

        if (lifeTime > maxLiftTime)
        {
            Destroy(this.gameObject);
        }

        if (isCombustion)
        {
            if (lifeTime < boostTime)
            {
                Combustion(boostPower);
            }
            else if (lifeTime < boostTime + sustainTime)
            {
                Combustion(sustainPower);
            }
            else
            {
                isCombustion = false;
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
}
