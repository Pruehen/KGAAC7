using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MWR : MonoBehaviour
{
    //List<Guided> incomeMissileList = new List<Guided>();
    bool isPlayer;

    int _missileCount = 0;

    [Header ("����")]
    [SerializeField] AudioSource WarningVoiceSfxPrefap;
    [SerializeField] AudioSource WarningBeepSfxPrefap;
    AudioSource WarningVoiceSfx;
    AudioSource WarningBeepSfx;

    private void Start()
    {
        isPlayer = GetComponent<AircraftMaster>().IsPlayer();
        WarningVoiceSfx = Instantiate(WarningVoiceSfxPrefap, transform).GetComponent<AudioSource>();
        WarningBeepSfx = Instantiate(WarningBeepSfxPrefap, transform).GetComponent<AudioSource>();
    }

    /// <summary>
    /// ����Ʈ�� �ڽſ��� �ٰ����� �̻����� �߰��ϴ� �޼���
    /// </summary>
    /// <param name="missile"></param>
    public void AddMissile(Guided missile)
    {
        //incomeMissileList.Add(missile);

        if(isPlayer)
        {
            MissileIndicatorController.Instance.AddMissileIndicator(missile);
            MissileCountAdd();
        }
    }

    /// <summary>
    /// ����Ʈ���� �ڽſ��� �ٰ����� �̻����� �����ϴ� �޼���
    /// </summary>
    /// <param name="missile"></param>
    public void RemoveMissile(Guided missile)
    {
        //incomeMissileList.Remove(missile);

        if (isPlayer)
        {
            MissileIndicatorController.Instance.RemoveMissileIndicator(missile);
            MissileCountRemove();
        }
    }

    private void MissileCountAdd()
    {
        _missileCount++;
        CheckAnyTracing();
    }
    private void MissileCountRemove()
    {
        _missileCount--;
        CheckAnyTracing();
    }

    private void CheckAnyTracing()
    {
        if(_missileCount > 0)
        {
            WarningVoiceSfx?.Stop();
            WarningBeepSfx?.Stop();
            WarningVoiceSfx?.Play();
            WarningBeepSfx?.Play();
        }
        else
        {
            WarningVoiceSfx?.Pause();
            WarningBeepSfx?.Pause();
        }
    }
}
