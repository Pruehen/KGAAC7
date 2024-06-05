using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MWR : NetworkBehaviour
{
    //List<Guided> incomeMissileList = new List<Guided>();
    bool isPlayer;

    private int _missileCount = 0;
    public int missileCount {
        get
        {
            return _missileCount;
        }
        private set
        {
            _missileCount = value;
            RpcSetMissileCount(value);
        }
            }

    private void RpcSetMissileCount(int value)
    {
        _missileCount = value;
    }

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
        if(isServer)
        {
            missileCount++;
            RpcAddMissile(missile.netId);
        }
    }

    [ClientRpc]
    private void RpcAddMissile(uint missile)
    {

        //incomeMissileList.Add(missile);
        missileCount++;
        if (isPlayer)
        {
            if (isLocalPlayer)
            {
                MissileIndicatorController.Instance.AddMissileIndicator(NetworkClient.spawned[missile].GetComponent<Guided>());
            }
            MissileCountAdd();
        }
    }

    /// <summary>
    /// ����Ʈ���� �ڽſ��� �ٰ����� �̻����� �����ϴ� �޼���
    /// </summary>
    /// <param name="missile"></param>
    public void RemoveMissile(Guided missile)
    {
        if(isServer)
        {
            missileCount--;
            RpcRemoveMissile(missile.netId);
        }
    }

    [ClientRpc]
    private void RpcRemoveMissile(uint missile)
    {
        //incomeMissileList.Remove(missile);
        missileCount--;
        if (isPlayer)
        {
            MissileIndicatorController.Instance.RemoveMissileIndicator(NetworkClient.spawned[netId].GetComponent<Guided>());
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
        if(!isLocalPlayer)
        {
            return;
        }
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
