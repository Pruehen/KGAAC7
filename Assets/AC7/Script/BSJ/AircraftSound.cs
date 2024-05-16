using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftSound : MonoBehaviour
{
    [SerializeField] GameObject _engineSoundPrefab;
    [SerializeField] GameObject _afterbunerSoundPrefab;
    private AudioSource _engineSound;
    private AudioSource _afterbunerSound;
    [SerializeField] AircraftMaster _aircraftMaster;

    private void Awake()
    {
        _engineSound = bsj.SoundManager.Instance.PlayAttached(_engineSoundPrefab, transform, true).GetComponent<AudioSource>();
        _engineSound.volume = .5f;
        _afterbunerSound = bsj.SoundManager.Instance.PlayAttached(_afterbunerSoundPrefab, transform, true).GetComponent<AudioSource>();
        _afterbunerSound.volume = 0f;
        _aircraftMaster = GetComponent<AircraftMaster>();
    }

    private void Update()
    {
        //UpdateEngineSound(Mathf.Min(_flightController.aircraftControl.throttle * 1000f + .2f, 1f));
        Debug.Log(_aircraftMaster.aircraftControl.throttle);
    }

    private void UpdateEngineSound(float power)
    {
        _afterbunerSound.volume = power;
    }
}
