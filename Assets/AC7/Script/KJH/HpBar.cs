using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : NetworkBehaviour
{
    Image hpBar;
    Combat playerCombat;
    float maxHp;
    // Start is called before the first frame update
    private void Start()
    {
        if(isLocalPlayer)
        {
            bsj.GameManager.Instance.AfterPlayerSpawned += OnPlayerSpawn;
        }
    }
    private void OnPlayerSpawn()
    {
        hpBar = GetComponent<Image>();
        playerCombat = bsj.GameManager.Instance.player.GetComponent<VehicleCombat>().Combat();
        StartCoroutine(DelayedGet());
    }
    private IEnumerator DelayedGet()
    {
        yield return new WaitForSeconds(.3f);
        maxHp = playerCombat.GetMaxHp();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerCombat == null) 
        { 
            return; 
        }
        float hpRatio = playerCombat.GetHp() / maxHp;
        hpBar.color = new Color(1, hpRatio, hpRatio);
    }
}
