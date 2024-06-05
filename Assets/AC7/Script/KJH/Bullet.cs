using Mirror;
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
        [SerializeField] GameObject _waterHitVfx;


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

        private void OnCollisionEnter(Collision collision)
        {
            VehicleCombat fightable;
            GenerateBullePassSfx bulletPassing;
            if (collision.transform.TryGetComponent<VehicleCombat>(out fightable))
            {
                fightable.TakeDamage(GetComponent<WeaponData>().Dmg());
                GameObject vsfx = ObjectPoolManager.Instance.DequeueObject(_bulletHitVfx);

                if (fightable.isPlayer && fightable.isLocalPlayer)
                {
                    kjh.GameManager.Instance.cameraShake.BulletHitShake();
                }

                //Vector3 playerToBullet = (-other.transform.position + transform.position).normalized;
                //vsfx.transform.position = other.transform.position + playerToBullet * Random.Range(10f, 22f) + other.transform.forward * 4f;
                vsfx.transform.position = collision.GetContact(0).point;
            }
            else
            {
                if (collision.transform.CompareTag("Water"))
                {
                    Vector3 contact = collision.GetContact(0).point;
                    EffectManager.Instance.EffectGenerate(_waterHitVfx, contact);
                }
                else if (collision.transform.CompareTag("Ground"))
                {
                    Vector3 contact = collision.GetContact(0).point;
                    EffectManager.Instance.EffectGenerate(_bulletHitVfx, contact);
                }
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
