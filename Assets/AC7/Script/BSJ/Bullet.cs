using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bsj
{
    public class Bullet : MonoBehaviour
    {
        private Rigidbody _rb;
        [SerializeField] public float _bulletSpeed = 1050f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
        public void Init()
        {
            _rb.velocity = transform.forward * _bulletSpeed;
        }
    }
}