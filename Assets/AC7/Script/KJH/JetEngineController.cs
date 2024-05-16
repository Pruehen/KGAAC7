using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetEngineController : MonoBehaviour
{
    ParticleSystem.MainModule particleMainModule;
    float initAlpha;
    float throttleAmount;
    Color particleColor;

    [Header("Common")]
    [SerializeField]
    float accelLerpAmount;
    [SerializeField]
    float brakeLerpAmount;

    [Header("Sounds")]
    [SerializeField]
    float maxVolume = 1f;
    [SerializeField]
    float lowpassValue = 2500;
    [SerializeField] AircraftEngineSound _engineAudio;

    float inputValue;
    public float InputValue
    {
        set { inputValue = value; }
    }

    void SetEngineAudio()
    {
        _engineAudio.UpdateAfterBurnerVolum(throttleAmount * maxVolume);
    }

    public void SetAudioEffect(bool is1stView)
    {
        _engineAudio.SetLowpassCutoff( (is1stView == true) ? lowpassValue : 22000);
    }

    void OnDisable()
    {
        throttleAmount = 0;
        particleColor.a = 0;

        particleMainModule.startColor = particleColor;
        if(_engineAudio != null) SetEngineAudio();
    }

    // Start is called before the first frame update
    void Awake()
    {
        _engineAudio = GetComponent<AircraftEngineSound>();

        particleMainModule = GetComponent<ParticleSystem>().main;
        particleColor = particleMainModule.startColor.color;
        initAlpha = particleColor.a;
        throttleAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float lerpAmount = (throttleAmount > inputValue) ? brakeLerpAmount : accelLerpAmount;
        throttleAmount = Mathf.Lerp(throttleAmount, inputValue, lerpAmount * Time.deltaTime);
        particleColor.a = throttleAmount * initAlpha;
        particleMainModule.startColor = particleColor;

        if(_engineAudio != null) SetEngineAudio();
    }
}
