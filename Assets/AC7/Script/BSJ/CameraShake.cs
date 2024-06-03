using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // 흔들림의 강도와 지속시간 설정
    [SerializeField] float _shakeDuration = 0.5f;
    [SerializeField] AnimationCurve _shakeCurve;
    [SerializeField] float _shakeMagnitudeV = 0.2f;
    [SerializeField] float _shakeMagnitudeH = 0.2f;
    [SerializeField] float _dampingSpeed = 1.0f;

    private Vector3 _initialPosition;
    private float _duration = 0f;
    private float _shakeTime;
    private VehicleCombat _player;

    private void Start()
    {
        bsj.GameManager.Instance.AfterPlayerSpawned += OnPlayerSpawn;
    }
    private void OnPlayerSpawn()
    {
        _player = bsj.GameManager.Instance.player.GetComponent<VehicleCombat>();
        _initialPosition = transform.localPosition;
        //StartCoroutine(DebugShake());
    }

    public void BulletHitShake()
    {
        TriggerShake(.2f, .3f, .1f);
    }

    public void MissileHitShake()
    {
        TriggerShake(.5f, .6f, .2f);
    }
    public void TriggerShake(float duration, float verticalForce, float horizontalForce)
    {
        //현재 실행중이면서 그 크기가 크면 취소
        if(_shakeMagnitudeV > verticalForce && IsShaking())
        {
            return;
        }
        _shakeTime = 0f;
        _duration = duration;
        _shakeMagnitudeV = verticalForce;
        _shakeMagnitudeH = horizontalForce;
    }

    void LateUpdate()
    {
        if(_player == null) 
        {
            return;
        }
        if (_player.IsDead())
        {
            return;
        }
        if (IsShaking())
        {
            float shakeX = _shakeCurve.Evaluate(_shakeTime / _duration * _shakeMagnitudeH * Random.Range(-1f, 1f));
            float shakeY = _shakeCurve.Evaluate(_shakeTime / _duration * _shakeMagnitudeV * Random.Range(-1f, 1f));
            transform.localPosition = new Vector3(_initialPosition.x + shakeX, _initialPosition.y + shakeY, _initialPosition.z);
            _shakeTime += Time.deltaTime;
        }
        else
        {
                transform.localPosition = _initialPosition;
        }
    }

    public bool IsShaking()
    {
        return _shakeTime < _duration;
    }
}