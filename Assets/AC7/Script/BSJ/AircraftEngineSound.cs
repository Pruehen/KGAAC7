using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftEngineSound : MonoBehaviour
{
    [SerializeField] GameObject _engineSoundPrefab;
    [SerializeField] GameObject _afterbunerSoundPrefab;
    private AudioSource _engineSound;
    private AudioLowPassFilter engineLowpass;
    private AudioSource afterbunerSound;
    private AudioLowPassFilter afterbunerLowpass;

    private void Awake()
    {
        _engineSound = bsj.SoundManager.Instance.PlayAttached(_engineSoundPrefab, transform, true).GetComponent<AudioSource>();
        _engineSound.volume = 1f;
        afterbunerSound = bsj.SoundManager.Instance.PlayAttached(_afterbunerSoundPrefab, transform, true).GetComponent<AudioSource>();
        afterbunerSound.volume = 0f;
        afterbunerLowpass = GetComponent<AudioLowPassFilter>();
    }
    /// <summary>
    /// 엔진 소리 크기 조절 0~1
    /// </summary>
    /// <param name="power"></param>
    public void UpdateEngineVolum(float power)
    {
        _engineSound.volume = power;
    }
    /// <summary>
    /// 애프터버너 소리 크기 조절 0~1
    /// </summary>
    /// <param name="power"></param>
    public void UpdateAfterBurnerVolum(float power)
    {
        afterbunerSound.volume = power;
    }
    /// <summary>
    /// 엔진과 애프터버너의 로우패스를 수정함
    /// </summary>
    /// <param name="amount"></param>
    public void SetLowpassCutoff(float amount)
    {
        engineLowpass.cutoffFrequency = amount;
        afterbunerLowpass.cutoffFrequency = amount;
    }
}
