using Mirror.Examples.RigidbodyBenchmark;
using Org.BouncyCastle.Security;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiAirtMoveTest : MonoBehaviour
{
    [SerializeField] private bool _dynamicMove;
    [SerializeField] private Vector3 _initialPos;
    Rigidbody _rb;
    public float dist = 80f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _initialPos = transform.position;
    }


    public Vector3 direction = Vector3.right;
    public float speed = 10f;
    void Update()
    {
        if(!_dynamicMove)
        {
            if(Vector3.Distance(_initialPos, transform.position) > dist)
            {
                direction = (-transform.position + _initialPos).normalized;
            }
            _rb.velocity = direction * speed;
        }
        else
        {
            Vector3 toInitialPos = (-transform.position + _initialPos + Vector3.up * 10f).normalized;

            _rb.AddForce(toInitialPos * speed / 10f, ForceMode.VelocityChange);
        }
    }
}
