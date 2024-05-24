using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedShip : MonoBehaviour
{
    Rigidbody _rb;
    [SerializeField] float _randXcenterMass;
    [SerializeField] float _randYcenterMass;
    [SerializeField] float _randZcenterMass;

    [SerializeField] AnimationCurve _sinkCurve;
    [SerializeField] float _sinkRatio;
    [SerializeField] float _sinkTime;


    private void Awake()
    {
        _rb= GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        float x = Random.Range(-_randXcenterMass / 2f, _randXcenterMass / 2f);
        float y = Random.Range(-_randYcenterMass / 2f, _randYcenterMass / 2f);
        float z = Random.Range(-_randZcenterMass / 2f, _randZcenterMass / 2f);
        Vector3 _randTorque = new Vector3(x, y, z);
        _rb.AddTorque(_randTorque, ForceMode.VelocityChange);
    }
    float curveTimer;
    private void FixedUpdate()
    {
        curveTimer += Time.fixedDeltaTime;
        _rb.velocity = -Vector3.up * _sinkCurve.Evaluate(curveTimer / _sinkTime) * _sinkRatio;
    }
}
