using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flare : MonoBehaviour
{
    Rigidbody rigidbody;
    ParticleSystem particleSystem;
    [SerializeField] GameObject _DeploySfx;
    [SerializeField] float maxTime;
    float lifeTime;

    public void Init(Vector3 position, Vector3 velocity)
    {
        lifeTime = 0;
        rigidbody = GetComponent<Rigidbody>();
        particleSystem = GetComponent<ParticleSystem>();

        this.transform.position = position;
        rigidbody.velocity = velocity;        
        particleSystem.Clear();
        particleSystem.Play();
        bsj.SoundManager.Instance.PlayAttached(_DeploySfx,transform);
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        if(lifeTime > maxTime)
        {
            ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
        }
    }
}
