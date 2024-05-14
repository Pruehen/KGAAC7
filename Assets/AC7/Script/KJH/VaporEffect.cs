using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaporEffect : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> strakeVaperEffect;
    [SerializeField] List<ParticleSystem> wingVaperEffect;

    bool alpha7Over = false;
    bool alpha20Over = false;
    private void Start()
    {
        alpha7Over = false;
        alpha20Over = false;
    }

    /// <summary>
    /// 항공기의 수증기 응축 이펙트를 생성하거나 제거해주는 메서드
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="aoa"></param>
    public void SetEffect(float speed, float aoa)
    {
        if(speed > 80)
        {
            if (alpha7Over == false && aoa > 7)
            {
                alpha7Over = true;
                SetParticleEffect(strakeVaperEffect, true);
            }
            if (alpha7Over == true && aoa < 7)
            {
                alpha7Over = false;
                SetParticleEffect(strakeVaperEffect, false);
            }
            if(alpha20Over == false && aoa > 20)
            {
                alpha20Over = true;
                SetParticleEffect(wingVaperEffect, true);
            }
            if (alpha20Over == true && aoa < 20)
            {
                alpha20Over = false;
                SetParticleEffect(wingVaperEffect, false);
            }
        }
        else
        {
            if(alpha7Over)
            {
                alpha7Over = false;
                SetParticleEffect(strakeVaperEffect, false);
            }
            if (alpha20Over) 
            {
                alpha20Over = false;
                SetParticleEffect(wingVaperEffect, false);
            }
        }
    }

    void SetParticleEffect(List<ParticleSystem> particleSystems, bool play)
    {
        foreach (ParticleSystem effect in particleSystems)
        {
            if(play)
            {
                effect.Play();
            }            
            else
            {
                effect.Stop();
            }
        }
    }
}
