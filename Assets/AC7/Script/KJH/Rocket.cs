using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("���� ���� �ð�")]
    [SerializeField] float boostTime;//�ν�Ʈ ���� �ð�
    [SerializeField] float sustainTime;//�������� ���� �ð�

    [Header("���� �߷�")]
    [SerializeField] float boostPower;//�ν�Ʈ ���� ���ӷ�
    [SerializeField] float sustainPower;//�������� ���� ���ӷ�

    [Header("FM")]
    [SerializeField] float liftPower;//���
    float cD;//�׷°�� (��°� �����)

    [SerializeField] float maxLiftTime;//�ִ� �۵� �ð�. �ʰ��� ����

    float lifeTime = 0;
    Rigidbody rigidbody;
    public bool isCombustion { get; private set; }//���� ���� ������ �Ǻ�
    Vector3 sideForce;
    /// <summary>
    /// ���� ������ �ް� �ִ� �����ӵ�(Vector3)�� ��ȯ��.
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
