using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AntiAirController_AI : MonoBehaviour
{
    VehicleMaster _owner;
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
    float _bulletSpeed = 1030f;

    /// <summary>
    /// 총구 위치
    /// </summary>
    [SerializeField] Transform _muzzleTrf;
    [SerializeField] Transform _gunTrf;
    [SerializeField] Transform _turretTrf;
    [SerializeField] bool _customTarget;
    [SerializeField] Rigidbody _target;

    IAntiAirWeapon _weapon;
    private SmoothRotation _muzzleRotator;

    Vector3 prevVel;

    [SerializeField] float _maxRotationSpeed = 30f;
    [SerializeField] float _smoothDuration = 1f;

    private void Start()
    {
        //bsj.GameManager.Instance.OnPlayerSpawned += Init;
        Init();
    }
    private void Init()
    {
        _owner = GetComponent<VehicleMaster>();
        _target = (_customTarget) ? _target : kjh.GameManager.Instance.player.GetComponent<Rigidbody>();
        prevVel = _target.velocity;
        _muzzleRotator = _muzzleTrf.GetComponent<SmoothRotation>();
        if (_muzzleRotator == null)
        {
            _muzzleRotator = _muzzleTrf.AddComponent<SmoothRotation>();
        }
        _muzzleRotator.Init(_maxRotationSpeed, _smoothDuration);
        _weapon = GetComponent<IAntiAirWeapon>();
    }
    void Update()
    {
        if (IsInRange() && !_owner.IsDead)
        {
            //타겟 적중을 위한 각도 계산
            Vector3 deltaVel = _target.velocity - prevVel;
            _desiredDirection = FireControlSystem.
                CalcFireDirection(_muzzleTrf.position, _target, _bulletSpeed, _accuracyLoop, deltaVel);
            
            //사격을 위해 회전
            Quaternion targetRot = Quaternion.LookRotation(_desiredDirection);
            Debug.DrawRay(_muzzleTrf.position,_desiredDirection * 100f ,Color.red);
            _muzzleRotator.UpdateTargetRotation(targetRot);
            Quaternion resultRotation = _muzzleRotator.GetRotation();
            _turretTrf.rotation = Quaternion.Euler(0f, resultRotation.eulerAngles.y, 0f);

            float verticalAngle = Mathf.Asin(_desiredDirection.y) * Mathf.Rad2Deg;

            //사격가능 각도인지 확인 후 조준 발사
            if (IsInAngle(verticalAngle))
            {
                _gunTrf.rotation = Quaternion.Euler(resultRotation.eulerAngles.x, _gunTrf.rotation.eulerAngles.y, resultRotation.eulerAngles.z);
                _weapon.Fire(true);
            }
            else
            {
                _weapon.Fire(false);
            }
        }
        else
        {
            _weapon.Fire(false);
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







    private static class FireControlSystem
    {
        /// <summary>
        /// 움직이는 표적을 맞추기 위한 방향을 계산
        /// </summary>
        /// <param name="originPosition">시작위치</param>
        /// <param name="target">타겟 리지드바디 </param>
        /// <param name="bulletSpeed">총알 속도</param>
        /// <param name="adjustAccurateLoop">정확도 루프 많이할수록 정확함</param>
        /// <returns></returns>
        /// 

        public static float GetTimeToTarget(Vector3 originPos, Vector3 targetPos, float speed)
        {
            return Vector3.Distance(targetPos, originPos) / speed;
        }

        public static Vector3 CalcFireDirection(Vector3 originPosition, Rigidbody target, float bulletSpeed, int accLoop, Vector3 velDelta)
        {
            Vector3 ToTarget = (-originPosition + CalcPredictTargetPos(originPosition, target, bulletSpeed, accLoop, velDelta)).normalized;
            return ToTarget;
        }


        public static Vector3 CalcPredictTargetPos(Vector3 originPosition, Rigidbody target, float bulletSpeed, int accLoop, Vector3 velDelta)
        {
            float timeToTarget = 0f;
            Vector3 resultPredictedPos = target.position;
            for (int i = 0; i < accLoop; i++)
            {
                //타겟 위치까지 도착 시간
                timeToTarget = GetTimeToTarget(originPosition, resultPredictedPos, bulletSpeed);
                //예측한 위치
                // Predicted position based on kinematic equation: s = ut + 0.5at^2
                resultPredictedPos = target.position + (target.velocity * timeToTarget) + (.5f * velDelta * timeToTarget * timeToTarget);
            }
            return resultPredictedPos;
        }
    }
}


