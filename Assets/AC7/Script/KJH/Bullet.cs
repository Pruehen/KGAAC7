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
        [SerializeField] GameObject _bulletHitVfx;

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
            VehicleCombat fightable;
            GenerateBullePassSfx bulletPassing;
            if (other.transform.TryGetComponent<VehicleCombat>(out fightable))
            {
                fightable.TakeDamage(GetComponent<WeaponData>().Dmg());
                GameObject vsfx = ObjectPoolManager.Instance.DequeueObject(_bulletHitVfx);

                if(fightable.isPlayer)
                {
                    kjh.GameManager.Instance.cameraShake.BulletHitShake();
                }

                //Vector3 playerToBullet = (-other.transform.position + transform.position).normalized;
                //vsfx.transform.position = other.transform.position + playerToBullet * Random.Range(10f, 22f) + other.transform.forward * 4f;
                vsfx.transform.position = other.transform.position;
            }
            if(other.TryGetComponent<GenerateBullePassSfx>(out bulletPassing))
            {
                return;
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
