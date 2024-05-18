using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 게임매니저를 통해 플레이어를 가져오가
/// 공격가능한 거리와 각도면 공격함
/// </summary>
public class M163 : MonoBehaviour
{
    /// <summary>
    /// 정확한 조준을 위한 반복 계산
    /// 4번만해도 충분
    /// </summary>
    [SerializeField] int _accuracyLoop = 4;
    [SerializeField] float _range = 2000f;

    [SerializeField] float _minXAxis = 0f;
    [SerializeField] float _maxXAxis = 180f;

    /// <summary>
    /// 락온시 조준 방향
    /// </summary>
    Vector3 _desiredDirection;

    /// <summary>
    /// 총구 위치
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
            //타겟 적중을 위한 각도 계산
            Vector3 deltaVel = _target.velocity - prevVel;
            _desiredDirection = FireControlSystem.CalcFireDirection(_muzzleTrf.position, _target, _bulletSpeed, _accuracyLoop, deltaVel);
            Quaternion targetRot = Quaternion.LookRotation(_desiredDirection);


            _muzzleRotator.UpdateTargetRotation(targetRot);

            Quaternion resultRotation = _muzzleRotator.GetRotation();

            _turretTrf.rotation = Quaternion.Euler(0f, resultRotation.eulerAngles.y, 0f);

            float verticalAngle = Mathf.Asin(_desiredDirection.y) * Mathf.Rad2Deg;

            //사격가능 각도인지 확인 후 조준 발사
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

