using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MWR : MonoBehaviour
{
    //List<Guided> incomeMissileList = new List<Guided>();
    bool isPlayer;

    public int missileCount { get; private set; }

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
        missileCount = 0;
    }

    /// <summary>
    /// ����Ʈ�� �ڽſ��� �ٰ����� �̻����� �߰��ϴ� �޼���
    /// </summary>
    /// <param name="missile"></param>
    public void AddMissile(Guided missile)
    {
        //incomeMissileList.Add(missile);
        missileCount++;
        if (isPlayer)
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
        missileCount--;
        if (isPlayer)
        {
            MissileIndicatorController.Instance.RemoveMissileIndicator(missile);
            MissileCountRemove();
        }
    }

    private void MissileCountAdd()
    {        
        CheckAnyTracing();
    }
    private void MissileCountRemove()
    {        
        CheckAnyTracing();
    }

    private void CheckAnyTracing()
    {
        if(missileCount > 0)
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
