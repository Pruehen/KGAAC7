using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SceneSingleton<EffectManager>
{
    public void EffectGenerate(GameObject item, Vector3 position)
    {
        GameObject effect = ObjectPoolManager.Instance.DequeueObject(item);
        effect.transform.position = position;
        effect.transform.rotation = Quaternion.identity;

        StartCoroutine(EffectEnqueue(effect, 5));
    }

    IEnumerator EffectEnqueue(GameObject item, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        ObjectPoolManager.Instance.EnqueueObject(item);
    }
}
