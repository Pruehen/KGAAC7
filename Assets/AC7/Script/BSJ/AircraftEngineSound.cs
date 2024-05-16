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
        _engineSound = bsj.SoundManager.Instance.PlayAttached(_engineSoundPrefab, transform, true).GetComponent<AudioSource>();
        _engineLowpass = _engineSound.GetComponent<AudioLowPassFilter>();
        _engineSound.volume = 1f * _volumRatio;
        _afterbunerSound = bsj.SoundManager.Instance.PlayAttached(_afterbunerSoundPrefab, transform, true).GetComponent<AudioSource>();
        _afterbunerLowpass = _afterbunerSound.GetComponent<AudioLowPassFilter>();
        _afterbunerSound.volume = 0f;
    }

    /// <summary>
    /// �����͹��� ���� ������Ʈ
    /// </summary>
    /// <param name="power"></param>
    public void SetAfterburnerVolum(float power)
    {
        _afterbunerSound.volume = power * _volumRatio;
    }
    /// <summary>
    /// 1��Ī�� �ƿ��� ��������
    /// </summary>
    /// <param name="amount"></param>
    public void SetCutoffFrequency(float amount)
    {
        _engineLowpass.cutoffFrequency = amount;
        _afterbunerLowpass.cutoffFrequency = amount;
    }
    /// <summary>
    /// ���� ���� ����
    /// </summary>
    /// <param name="ratio"></param>
    public void SetOverallVolumRatio(float ratio) 
    {
        _volumRatio = ratio;
    }
}
