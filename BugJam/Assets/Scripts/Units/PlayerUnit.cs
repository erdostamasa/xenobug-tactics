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


        Vector3 dir = (target.transform.position - turret.transform.position).normalized;

        Vector3 oldForward = turret.forward;

        Debug.DrawLine(turret.position, turret.position + turret.forward, Color.red, 5f);
        dir = new Vector3(dir.x, 0, dir.z);
        Debug.DrawLine(turret.position, turret.position + dir, Color.cyan, 5f);
        turret.forward = dir;

        float targetY = turret.localRotation.eulerAngles.y;

        turret.forward = oldForward;

        Debug.LogWarning("y target: " + targetY);
        StartCoroutine(RotateTurret(targetY));
        muzzleFlash.Play();

        target.TakeDamage(damage);
    }

    IEnumerator RotateTurret(float yDestination) {
        int maxIters = 1000;
        int i = 0;
        
        /*
         
         Math.Abs(turret.rotation.eulerAngles.y - yDestination) = 90,00003
         yDestination = 91,02198
         turretrot = 181.02201
         
         
         */
        
        while (Math.Abs(turret.localRotation.eulerAngles.y - yDestination) > 1f && i < maxIters) {
            print(Math.Abs(turret.localRotation.eulerAngles.y - yDestination) + " deltatime: " + Time.deltaTime + " destination: " + yDestination);
            turret.localEulerAngles = new Vector3(0, Mathf.LerpAngle(turret.localRotation.eulerAngles.y, yDestination, Time.deltaTime * 8f), 0);
            i++;
            yield return new WaitForEndOfFrame();
        }
    }

    public override void DestroySelf() {
        EventManager.instance.UnitDestroyed(this);
        //base.DestroySelf();
        explosionParticle.SetActive(true);
        smokeParticle.SetActive(true);
        currentTile.Unit = null;
        currentTile.walkable = false;
        currentTile.selectable = false;
    }
}