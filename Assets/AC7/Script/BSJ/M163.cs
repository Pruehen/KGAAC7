using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���ӸŴ����� ���� �÷��̾ ��������
/// ���ݰ����� �Ÿ��� ������ ������
/// </summary>
public class M163 : MonoBehaviour
{
    /// <summary>
    /// ��Ȯ�� ������ ���� �ݺ� ���
    /// 4�����ص� ���
    /// </summary>
    [SerializeField] int _accuracyLoop = 4;
    [SerializeField] float _range = 2000f;

    [SerializeField] float _minXAxis = 0f;
    [SerializeField] float _maxXAxis = 180f;

    /// <summary>
    /// ���½� ���� ����
    /// </summary>
    Vector3 _desiredDirection;

    /// <summary>
    /// �ѱ� ��ġ
    /// </summary>
    [SerializeField] Transform _muzzleTrf;
    Transform _turretTrf;
    Transform _gunTrf;
    Rigidbody _target;

    float _bulletSpeed;

    Vulcan _vulcan;


    private void Awake()
    {
        _turretTrf = transform.GetChild(1);
        _gunTrf = transform.GetChild(1).GetChild(0);
        _vulcan = _gunTrf.GetComponent<Vulcan>();
        bsj.GameManager.Instance.OnPlayerSpawned += Init;
    }

    private void Start()
    {
        _bulletSpeed = _vulcan.bulletSpeed;
        bsj.GameManager.Instance.OnPlayerSpawned += Init;
    }
    Vector3 prevVel;
    private void Init()
    {
        _target = bsj.GameManager.Instance.player.GetComponent<Rigidbody>();
        prevVel = _target.velocity;
    }
    void Update()
    {
        if(IsInRange())
        {
            Vector3 deltaVel = _target.velocity - prevVel;
            _desiredDirection = FireControlSystem.CalcFireDirection(_muzzleTrf.position, _target, _bulletSpeed, _accuracyLoop, deltaVel);
            Quaternion targetRot = Quaternion.LookRotation(_desiredDirection);

            _turretTrf.rotation = Quaternion.Euler(new Vector3(0f, targetRot.eulerAngles.y, 0f));
            float verticalAngle = Mathf.Asin(_desiredDirection.y) * Mathf.Rad2Deg;

            if(IsInAngle(verticalAngle))
            {
                _muzzleTrf.localRotation = Quaternion.Euler(new Vector3(-verticalAngle, 0, 0));
                _vulcan.Fire();
            }

        }
        prevVel = _target.velocity;
    }
    
    private bool IsInRange()
    {
        return Vector3.Distance(_target.position, transform.position) < _range;
    }

    private bool IsInAngle(float verticalAngle)
    {
        return verticalAngle >= _minXAxis && verticalAngle <= _maxXAxis;
    }
}
