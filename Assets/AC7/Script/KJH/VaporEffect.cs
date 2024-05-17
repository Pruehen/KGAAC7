using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaporEffect : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> strakeVaperEffect;
    [SerializeField] List<ParticleSystem> wingVaperEffect;

    bool alpha5Over = false;
    bool alpha10Over = false;
    private void Start()
    {
        alpha5Over = false;
        alpha10Over = false;
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
            if (alpha5Over == false && aoa > 5)
            {
                alpha5Over = true;
                SetParticleEffect(strakeVaperEffect, true);
            }
            if (alpha5Over == true && aoa < 5)
            {
                alpha5Over = false;
                SetParticleEffect(strakeVaperEffect, false);
            }
            if(alpha10Over == false && aoa > 10)
            {
                alpha10Over = true;
                SetParticleEffect(wingVaperEffect, true);
            }
            if (alpha10Over == true && aoa < 10)
            {
                alpha10Over = false;
                SetParticleEffect(wingVaperEffect, false);
            }
        }
        else
        {
            if(alpha5Over)
            {
                alpha5Over = false;
                SetParticleEffect(strakeVaperEffect, false);
            }
            if (alpha10Over) 
            {
                alpha10Over = false;
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
