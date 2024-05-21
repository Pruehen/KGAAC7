using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBullePassSfx : MonoBehaviour
{
    [SerializeField] GameObject _RandomBulletPassingSfx;
    [SerializeField] float _interval = .1f;
    private float _prevTimeStamp = 0f;
    private void OnTriggerExit(Collider other)
    {
        if (Time.time - _prevTimeStamp < _interval)
            return;
        _prevTimeStamp = Time.time;
        GameObject sfx = ObjectPoolManager.Instance.DequeueObject(_RandomBulletPassingSfx);
        sfx.transform.position = other.transform.position;
        ObjectPoolManager.Instance.EnqueueObject(sfx, 5f);
    }
}
