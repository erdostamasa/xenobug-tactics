using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit {
    [SerializeField] GameObject explosionParticle;
    [SerializeField] GameObject smokeParticle;
    [SerializeField] ParticleSystem muzzleFlash;
    
    

    [SerializeField] Transform turret;

    public override void AttackAnimate(Unit target) {
        //base.AttackAnimate(target);
        GameManager.instance.moveInProgress = true;

        Vector3 dir = (target.transform.position - turret.transform.position).normalized;
        Vector3 oldForward = turret.forward;
        dir = new Vector3(dir.x, 0, dir.z);
        turret.forward = dir;
        float targetY = turret.localRotation.eulerAngles.y;
        turret.forward = oldForward;

        StartCoroutine(RotateTurret(targetY, target));
    }

    

    IEnumerator RotateTurret(float yDestination, Unit target) {
        int maxIters = 200;
        int i = 0;
        while (Math.Abs(turret.localRotation.eulerAngles.y - yDestination) > 1f && i < maxIters) {
            turret.localEulerAngles = new Vector3(0, Mathf.LerpAngle(turret.localRotation.eulerAngles.y, yDestination, Time.deltaTime * 5f), 0);
            //turret.localEulerAngles += new Vector3(0, Time.deltaTime * 5f, 0);
            i++;
            yield return new WaitForEndOfFrame();
        }

        muzzleFlash.Play();
        yield return new WaitForSeconds(0.1f);
        SoundManager.instance.PlaySound(attackSound);
        target.TakeDamage(damage);
        GameManager.instance.moveInProgress = false;
        SetUnavailable();
    }

    public override void DestroySelf() {
        SoundManager.instance.PlaySound(deathSound);
        EventManager.instance.UnitDestroyed(this);
        //base.DestroySelf();
        explosionParticle.SetActive(true);
        
        
        smokeParticle.SetActive(true);
        currentTile.Unit = null;
        currentTile.walkable = false;
        currentTile.selectable = false;
        Destroy(turret.gameObject);
    }
}