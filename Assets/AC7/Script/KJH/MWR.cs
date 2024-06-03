using Mirror;
using System.Collections;
using UnityEngine;

public class MWR : NetworkBehaviour
{
    //List<Guided> incomeMissileList = new List<Guided>();
    bool isPlayer;

    public int missileCount { get; private set; }

    [Header ("����")]
    [SerializeField] AudioSource WarningVoiceSfxPrefap;
    [SerializeField] AudioSource WarningBeepSfxPrefap;
    AudioSource WarningVoiceSfx;
    AudioSource WarningBeepSfx;
    [SerializeField] float _missileWarnigSfxDelay = 1f;

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
        if (isPlayer && this.isLocalPlayer)
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
    Coroutine sfxLoop;
    private void CheckAnyTracing()
    {
        if(missileCount > 0)
        {
            if (sfxLoop == null)
            {
                sfxLoop = StartCoroutine(AlertSfxLoop(WarningVoiceSfx, _missileWarnigSfxDelay));
                WarningBeepSfx?.Play();
            }
        }
        else
        {
            if(sfxLoop != null)
            {
                WarningBeepSfx?.Pause();
                StopCoroutine(sfxLoop);
                sfxLoop = null;
            }
        }
    }

    private IEnumerator AlertSfxLoop(AudioSource audioSource, float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            audioSource.Play();
        }
    }
}
