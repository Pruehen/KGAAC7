using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager3 : MonoBehaviour
{
    [SerializeField] List<GameObject> _enemyList;
    [SerializeField] List<float> _spawnCoolTimeList;        

    float playTime = 0;
    int iterator = 0;

    private void Update()
    {
        if (iterator < _enemyList.Count && iterator < _spawnCoolTimeList.Count)
        {
            playTime += Time.deltaTime;

            if (_spawnCoolTimeList[iterator] < playTime)
            {
                Spawn(iterator);
                iterator++;
            }
        }
    }

    public void Spawn(int index)
    {
        GameObject enemySqr = _enemyList[index];
        enemySqr.SetActive(true);
        VehicleCombat[] VehicleItem = enemySqr.GetComponentsInChildren<VehicleCombat>();
        foreach (var item in VehicleItem)
        {
            kjh.GameManager.Instance.AddActiveTarget(item);

            //���� ��� ������ ������ �����Ƿ� �ڽ��� ã�Ƽ� �߰�
            VehicleCombat[] childCombat = item.GetComponentsInChildren<VehicleCombat>();
            foreach (VehicleCombat childCombatItem in childCombat)
            { kjh.GameManager.Instance.AddActiveTarget(childCombatItem); }
        }
        Debug.Log("Spawned");
    }
}
