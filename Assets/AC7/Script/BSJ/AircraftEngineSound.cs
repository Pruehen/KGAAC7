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
    /// ���� �Ҹ� ũ�� ���� 0~1
    /// </summary>
    /// <param name="power"></param>
    public void UpdateEngineVolum(float power)
    {
        _engineSound.volume = power;
    }
    /// <summary>
    /// �����͹��� �Ҹ� ũ�� ���� 0~1
    /// </summary>
    /// <param name="power"></param>
    public void UpdateAfterBurnerVolum(float power)
    {
        afterbunerSound.volume = power;
    }
    /// <summary>
    /// ������ �����͹����� �ο��н��� ������
    /// </summary>
    /// <param name="amount"></param>
    public void SetLowpassCutoff(float amount)
    {
        engineLowpass.cutoffFrequency = amount;
        afterbunerLowpass.cutoffFrequency = amount;
    }
}
