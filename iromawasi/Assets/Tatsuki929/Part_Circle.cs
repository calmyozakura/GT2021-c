using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part_Circle : MonoBehaviour
{
    ParticleSystem p_ParticleSystem;
    
    private ParticleSystem.Particle[] p_Particle;

    //開始時
    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);//乱数シードの初期化

        p_ParticleSystem = GetComponent<ParticleSystem>();
        
        
    }

    
    //更新
    void Update()
    {
        

        p_ParticleSystem.startSize = Random.Range(0.33f, 0.66f);

        int maxParticles = 1000;

        if (p_Particle == null || p_Particle.Length < maxParticles)
        {
            p_Particle = new ParticleSystem.Particle[maxParticles];
        }        
       

    }
}
//理解していること
/*
 * ParticleSystem.Particle.VelocityOverLifetime.OrbitalZに-1を掛けると回転を反転できる
 * しかし、VelocityOverLifetimeがset出来ないため-1を掛けることができず
 * 現状ではパーティクル自身のYを反転させることが有力
 */