using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private IFightable _owner;


    public void Init(IFightable owner)
    {
        _owner = owner;
    }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        _rigidbody.velocity = transform.forward * 100f;
    }
    private void OnCollisionEnter(Collision collision)
    {
        bool isEnemy = collision.gameObject.CompareTag("Enemy");
        if(isEnemy)
        {
            //적이면 공격해야지
            IFightable enemy = collision.gameObject.transform.GetComponent<Enemy>();
            _owner.DealDamage(enemy, 10f);
        }
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }
}
