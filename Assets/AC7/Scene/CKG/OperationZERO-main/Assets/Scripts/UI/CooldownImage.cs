using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownImage : MonoBehaviour
{
    public Image frameImage;
    public Image fillImage;

    float remainCooldown;
    float maxCooldown;
    float cooldownReciprocal;

    kjh.WeaponSystem weaponSystem;

    public void SetWeaponData(kjh.WeaponSystem weaponSlot, Sprite frameSprite, Sprite fillSprite)
    {
        weaponSystem = weaponSlot;
        frameImage.sprite = frameSprite;
        fillImage.sprite = fillSprite;
    }

    public void SetColor(Color color)
    {
        frameImage.color = color;
        fillImage.color = color;
    }

    public void StartCooldown(float cooldown)
    {
        maxCooldown = cooldown;
        remainCooldown = 0;
        cooldownReciprocal = 1 / cooldown;
    }

    // Start is called before the first frame update
    void Start()
    {
        remainCooldown = maxCooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //fillImage.fillAmount = weaponSystem.MslCoolDownRatio();
    }
}
