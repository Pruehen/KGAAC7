using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoolObjectOnDestroy : MonoBehaviour
{
    [SerializeField] GameObject _randomWaterSplashVfx;
    [SerializeField] float _scale;
    // Start is called before the first frame update

    public void Spawn()
    {
        GameObject item = ObjectPoolManager.Instance.DequeueObject(_randomWaterSplashVfx);
        item.transform.localScale = Vector3.one * _scale;
        Vector3 onWaterPos = transform.position;
        onWaterPos.y = 0f;
        item.transform.position = onWaterPos;
        ObjectPoolManager.Instance.EnqueueObject(item, 300f);
    }
}
