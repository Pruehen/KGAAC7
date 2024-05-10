using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Plain : NetworkBehaviour
{
    private Rigidbody _rigidbody;
    private float _rollInput;
    private float _pitchInput;
    private float _rollSpeed = 2.0f;
    private float _thrustSpeed = 5.0f;
    private float _pitchSpeed = 1.0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if(!isServer)
        {
            UpdateInput();
            cmdUpdateInput(_rollInput, _pitchInput);
            return;
        }
        if (isLocalPlayer)
        {
            UpdateInput();
        }
        Roll(_rollInput);
        Pitch(_pitchInput);
        Thrust(1f);
    }

    [Command]
    private void cmdUpdateInput(float rollInput, float pitchInput)
    {
        _rollInput = rollInput;
        _pitchInput = pitchInput;
    }

    private void Roll(float input)
    {
        Vector3 res = _rollInput * transform.forward * _rollSpeed;
        _rigidbody.AddTorque(res, ForceMode.Impulse);
    }
    private void Pitch(float input)
    {
        Vector3 res = _pitchInput * transform.right * _pitchSpeed;
        _rigidbody.AddTorque(res, ForceMode.Impulse);
    }
    private void Thrust(float input)
    {
        _rigidbody.velocity = transform.forward * _thrustSpeed;
    }
    private float GetRollInput()
    {
        return -Input.GetAxis("Horizontal");
    }
    private float GetPitchInput()
    {
        return Input.GetAxis("Vertical");
    }
    private void UpdateInput()
    {
        _rollInput = GetRollInput();
        _pitchInput = GetPitchInput();
    }


}
