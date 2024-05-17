using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kjh
{
    public class Bullet : MonoBehaviour
    {
        SphereCollider sphereCollider;
        Rigidbody rigidbody;
        float lifeTime = 0;

        public void Init(Vector3 position, Vector3 velocity)
        {
            if(sphereCollider == null)
            {
                sphereCollider = GetComponent<SphereCollider>();
                rigidbody = GetComponent<Rigidbody>();
            }

            sphereCollider.enabled = false;
            lifeTime = 0;
            
            this.transform.position = position;
            rigidbody.velocity = velocity;

            GetComponent<TrailRenderer>().Clear();
        }

        // Update is called once per frame
        void Update()
        {
            lifeTime += Time.deltaTime;

            if (sphereCollider.enabled == false && lifeTime > 0.1f)
            {
                sphereCollider.enabled = true;
            }

            if(lifeTime > 3)
            {
                DestroyObject();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            IFightable fightable;
            if (other.transform.TryGetComponent<IFightable>(out fightable))
            {
                fightable.TakeDamage(GetComponent<WeaponData>().Dmg());
            }

            DestroyObject();
        }

        void DestroyObject()
        {
            GetComponent<TrailRenderer>().Clear();
            ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
        }
    }
}
