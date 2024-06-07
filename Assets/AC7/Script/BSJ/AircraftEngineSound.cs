using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftEngineSound : MonoBehaviour
{
    [SerializeField] GameObject _engineSoundPrefab;
    [SerializeField] GameObject _afterbunerSoundPrefab;
    private AudioSource _engineSound;
    private AudioLowPassFilter _engineLowpass;
    private AudioSource _afterbunerSound;
    private AudioLowPassFilter _afterbunerLowpass;
    private float _volumRatio = 1.0f;

    private void Awake()
    {
        _engineSound = bsj.SoundManager.Instance.SpawnAttached(_engineSoundPrefab, transform, true).GetComponent<AudioSource>();
        _afterbunerSound = bsj.SoundManager.Instance.SpawnAttached(_afterbunerSoundPrefab, transform, true).GetComponent<AudioSource>();
        _engineSound.volume = 1f * _volumRatio;
        _afterbunerSound.volume = 0f;
        _engineSound.Play();
        _afterbunerSound.Play();
        _engineLowpass = _engineSound.GetComponent<AudioLowPassFilter>();
        _afterbunerLowpass = _afterbunerSound.GetComponent<AudioLowPassFilter>();
    }
    private void OnEnable()
    {
    }
    /// <summary>
    /// 애프터버너 볼륨 업데이트
    /// </summary>
    /// <param name="power"></param>
    public void SetAfterburnerVolum(float power)
    {
        if (!_engineSound.isPlaying)
        {
            _engineSound.Play();
        }
        if (!_afterbunerSound.isPlaying)
        {
            _afterbunerSound.Play();
        }
        _engineSound.pitch = 1 + power;
        _afterbunerSound.volume = power * _volumRatio;
    }
    /// <summary>
    /// 1인칭용 컷오프 프리퀀시
    /// </summary>
    /// <param name="amount"></param>
    public void SetCutoffFrequency(float amount)
    {
        _engineLowpass.cutoffFrequency = amount;
        _afterbunerLowpass.cutoffFrequency = amount;
    }
    /// <summary>
    /// 엔진 볼륨 조절
    /// </summary>
    /// <param name="ratio"></param>
    public void SetOverallVolumRatio(float ratio) 
    {
        _volumRatio = ratio;
    }
}
