using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit {
    [SerializeField] protected GameObject deathParticlePrefab;
    [SerializeField] Transform deathParticlePosition;
    

    public override void DestroySelf() {
        GameObject dp = Instantiate(deathParticlePrefab, deathParticlePosition.position, deathParticlePrefab.transform.rotation);
        Destroy(dp, 5f);
        
        SoundManager.instance.PlaySound(deathSound);
        base.DestroySelf();
    }
}