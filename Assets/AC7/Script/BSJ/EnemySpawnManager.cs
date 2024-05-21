using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _sqrList;
    [SerializeField] Transform _enemyParent;

    Queue<GameObject> _sqrQueue = new Queue<GameObject>();

    [SerializeField] bool _isSpawning = true;
    [SerializeField] float _distance = 8000f;
    [SerializeField] int _startSpawnCount = 4;

    private void Awake()
    {
        foreach (var item in _sqrList)
        {
            _sqrQueue.Enqueue(item);
        }
    }
    private void Start()
    {
        if (_isSpawning)
            kjh.GameManager.Instance.targetCountChanged += OnTargetCountChanged;
    }

    //���� ���� ������?
    //���� ���� �Ÿ� �հ����� ����

    //��� ������ ť�� ���
    //ť���� ���鼭 ����
    //������ ��ŭ�� ����

    //�÷��̾������� �´�
    // �÷��̾���ġ���� �����Ÿ� ������ �Ÿ����� ����
    // �Ÿ��� ������ ����
    // Instantiate ���

    /// <summary>
    /// ���� ����, ����
    /// </summary>
    /// <param name="spawning"></param>
    public void SetSpawn(bool spawning)
    {
        _isSpawning = spawning;
    }

    public void SpawnAtRandom(float distance)
    {
        Vector3 playerPosition = kjh.GameManager.Instance.player.transform.position;
        Vector3 randomDir = new(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        Vector3 spawnPos = playerPosition + randomDir * distance;
        Quaternion rotation = Quaternion.LookRotation(-spawnPos + playerPosition);

        GameObject sqr = _sqrQueue.Dequeue();

        GameObject sqrItem = Instantiate(sqr, spawnPos, rotation, _enemyParent);
        VehicleCombat[] VehicleItem = sqrItem.GetComponentsInChildren<VehicleCombat>();
        foreach (var item in VehicleItem)
        {
            kjh.GameManager.Instance.AddActiveTarget(item);
        }
        Debug.Log("Spawned");
    }

    private void OnTargetCountChanged(int count)
    {
        if(count <= _startSpawnCount)
        {
            SpawnAtRandom(_distance);
        }
    }
}
