using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableSweepWing : MonoBehaviour
{
    [SerializeField] Transform leftWing;
    [SerializeField] Transform rightWing;
    [SerializeField] AnimationCurve wingAngleCurve; //가변익 각도 커브

    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = this.transform.parent.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rigidbody != null)
        {
            float speed = rigidbody.velocity.magnitude;
            rightWing.localRotation = Quaternion.Euler(0, wingAngleCurve.Evaluate(speed), 0);
            leftWing.localRotation = Quaternion.Euler(0, -wingAngleCurve.Evaluate(speed), 0);
        }
    }
}
