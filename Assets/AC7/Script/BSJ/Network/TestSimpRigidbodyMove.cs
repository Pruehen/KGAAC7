using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSimpRigidbodyMove : NetworkBehaviour
{
    [SerializeField] private Rigidbody Rigidbody_rb;

    [SyncVar]
    Vector3 _velocity;
    [SyncVar]
    Vector3 _position;


    private void Awake()
    {
        Rigidbody_rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float vertical = Input.GetAxis("Vertical");
        Rigidbody_rb.AddForce(Vector3.up * vertical, ForceMode.VelocityChange);

        if(isLocalPlayer)
        {
            _velocity = Rigidbody_rb.velocity;
            _position = transform.position;
        }
        else
        {
            Rigidbody_rb.velocity = _velocity;
            transform.position = _position;
        }
    }
}
