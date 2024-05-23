using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AntiAirController_AI : MonoBehaviour
{
    VehicleMaster _owner;
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
    float _bulletSpeed = 1030f;

    /// <summary>
    /// �ѱ� ��ġ
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
            //Ÿ�� ������ ���� ���� ���
            Vector3 deltaVel = _target.velocity - prevVel;
            _desiredDirection = FireControlSystem.
                CalcFireDirection(_muzzleTrf.position, _target, _bulletSpeed, _accuracyLoop, deltaVel);
            
            //����� ���� ȸ��
            Quaternion targetRot = Quaternion.LookRotation(_desiredDirection);
            Debug.DrawRay(_muzzleTrf.position,_desiredDirection * 100f ,Color.red);
            _muzzleRotator.UpdateTargetRotation(targetRot);
            Quaternion resultRotation = _muzzleRotator.GetRotation();
            _turretTrf.rotation = Quaternion.Euler(0f, resultRotation.eulerAngles.y, 0f);

            float verticalAngle = Mathf.Asin(_desiredDirection.y) * Mathf.Rad2Deg;

            //��ݰ��� �������� Ȯ�� �� ���� �߻�
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
        /// �����̴� ǥ���� ���߱� ���� ������ ���
        /// </summary>
        /// <param name="originPosition">������ġ</param>
        /// <param name="target">Ÿ�� ������ٵ� </param>
        /// <param name="bulletSpeed">�Ѿ� �ӵ�</param>
        /// <param name="adjustAccurateLoop">��Ȯ�� ���� �����Ҽ��� ��Ȯ��</param>
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
                //Ÿ�� ��ġ���� ���� �ð�
                timeToTarget = GetTimeToTarget(originPosition, resultPredictedPos, bulletSpeed);
                //������ ��ġ
                // Predicted position based on kinematic equation: s = ut + 0.5at^2
                resultPredictedPos = target.position + (target.velocity * timeToTarget) + (.5f * velDelta * timeToTarget * timeToTarget);
            }
            return resultPredictedPos;
        }
    }
}


