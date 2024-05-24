using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    Image hpBar;
    Combat playerCombat;
    float maxHp;
    // Start is called before the first frame update
    void Start()
    {
        hpBar = GetComponent<Image>();
        playerCombat = kjh.GameManager.Instance.player.vehicleCombat.Combat();
        maxHp = playerCombat.GetMaxHp();
    }

    // Update is called once per frame
    void Update()
    {
        float hpRatio = playerCombat.GetHp() / maxHp;
        hpBar.color = new Color(1, hpRatio, hpRatio);
    }
}
