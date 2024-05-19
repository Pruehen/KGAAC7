using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kjh
{
    public class Bullet : MonoBehaviour
    {
        SphereCollider sphereCollider;
        float lifeTime = 0;
        // Start is called before the first frame update
        void Start()
        {
            sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.enabled = false;
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
                Destroy(this.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            IFightable fightable;
            if (other.transform.TryGetComponent<IFightable>(out fightable))
            {
                fightable.TakeDamage(GetComponent<WeaponData>().Dmg());
                Destroy(this.gameObject);
            }

        }

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(this.gameObject);
        }
    }
}
