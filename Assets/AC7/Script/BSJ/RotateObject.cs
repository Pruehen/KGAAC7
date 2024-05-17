using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private bool _x = false;
    [SerializeField] private bool _y = false;
    [SerializeField] private bool _z = true;
    [SerializeField] private bool _inverce = false;
    [SerializeField] private float _speed = 10f;
    private void Update()
    {
        float speed = Time.deltaTime * _speed;
        float x = _x ? 1f : 0f;
        float y = _y ? 1f : 0f;
        float z = _z ? 1f : 0f;
        transform.Rotate(x * speed, y * speed, z * speed);
    }
}
