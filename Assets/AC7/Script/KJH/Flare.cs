using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flare : MonoBehaviour
{
    Rigidbody rigidbody;
    ParticleSystem particleSystem;
    [SerializeField] float maxTime;
    float lifeTime;

    private void Awake()
    {

    }

    public void Init(Vector3 position, Vector3 velocity)
    {
        rigidbody = GetComponent<Rigidbody>();
        particleSystem = GetComponent<ParticleSystem>();

        this.transform.position = position;
        rigidbody.velocity = velocity;
        particleSystem.Play();
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        if(lifeTime > maxTime)
        {
            Destroy(this.gameObject);
        }
    }
}
