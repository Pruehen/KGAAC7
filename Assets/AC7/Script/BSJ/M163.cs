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
    Transform _gunTrf;
    Transform _turretTrf;
    [SerializeField] bool _customTarget;
    [SerializeField] Rigidbody _target;

    float _bulletSpeed;

    Vulcan _vulcan;
    private SmoothRotation _muzzleRotator;

    bool _isDead;
    private void Awake()
    {
        _turretTrf = transform.GetChild(1);
        _gunTrf = transform.GetChild(1).GetChild(0);
        _vulcan = _gunTrf.GetComponent<Vulcan>();
        _muzzleRotator = _muzzleTrf.GetComponent<SmoothRotation>();
        //bsj.GameManager.Instance.OnPlayerSpawned += Init;
    }

    private void Start()
    {
        _bulletSpeed = _vulcan.bulletSpeed;
        //bsj.GameManager.Instance.OnPlayerSpawned += Init;
        Init();
    }
    Vector3 prevVel;
    private void Init()
    {
        _target = (_customTarget) ? _target : kjh.GameManager.Instance.player.GetComponent<Rigidbody>();
        prevVel = _target.velocity;
    }
    void Update()
    {
        if (IsInRange() && !_isDead)
        {
            //Ÿ�� ������ ���� ���� ���
            Vector3 deltaVel = _target.velocity - prevVel;
            _desiredDirection = FireControlSystem.CalcFireDirection(_muzzleTrf.position, _target, _bulletSpeed, _accuracyLoop, deltaVel);
            Quaternion targetRot = Quaternion.LookRotation(_desiredDirection);


            _muzzleRotator.UpdateTargetRotation(targetRot);

            Quaternion resultRotation = _muzzleRotator.GetRotation();

            _turretTrf.rotation = Quaternion.Euler(0f, resultRotation.eulerAngles.y, 0f);

            float verticalAngle = Mathf.Asin(_desiredDirection.y) * Mathf.Rad2Deg;

            //��ݰ��� �������� Ȯ�� �� ���� �߻�
            if (IsInAngle(verticalAngle))
            {
                _gunTrf.rotation = Quaternion.Euler(resultRotation.eulerAngles.x, _gunTrf.rotation.eulerAngles.y, resultRotation.eulerAngles.z);
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
    public void Dead()
    {
        _isDead = true;
        EffectManager.Instance.AircraftFireEffectGenerate(this.transform);
        StartCoroutine(DeadEffect());
    }

    IEnumerator DeadEffect()
    {
        yield return new WaitForSeconds(2.5f);
        EffectManager.Instance.AircraftExplosionEffectGenerate(this.transform.position);
        Destroy(this.gameObject);
    }
}

