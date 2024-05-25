using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoolObjectOnCombatDestroy : MonoBehaviour
{
    [SerializeField] GameObject _spawnObjectPrifab;
    [SerializeField] float _scale = 1f;
    [SerializeField] bool _onWater = false;
    [SerializeField] float _lifeTime = 300f;
    VehicleMaster _masterCombat;

    // Start is called before the first frame update

    private void Awake()
    {
        _masterCombat = GetComponent<VehicleMaster>();
        _masterCombat.OnCombatDestroy.AddListener(Spawn);
    }


    public void Spawn()
    {
        GameObject item = ObjectPoolManager.Instance.DequeueObject(_spawnObjectPrifab);
        item.transform.localScale = Vector3.one * _scale;
        item.transform.position = transform.position;
        if (_onWater)
        {
            Vector3 onWaterPos = transform.position;
            onWaterPos.y = 0f;
            item.transform.position = onWaterPos;
        }
        ObjectPoolManager.Instance.EnqueueObject(item, _lifeTime);
    }
}
