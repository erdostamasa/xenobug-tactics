using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugEnemy : Unit {
    public override void AttackAnimate(Unit target) {
        GameManager.instance.moveInProgress = true;
        //base.AttackAnimate(target);


        // line.EndPos = target.transform.position;
        StartCoroutine(AttackAnimation(target.currentTile));
    }

    IEnumerator AttackAnimation(Tile target) {
        //turn towards target
        Vector3 dir = (target.unitPosition.position - currentTile.unitPosition.position).normalized;
        Vector3 oldForward = transform.forward;

        transform.forward = dir;
        float targetY = transform.localRotation.eulerAngles.y;
        transform.forward = oldForward;

        int maxIters = 200;
        int i = 0;
        while (Math.Abs(transform.localRotation.eulerAngles.y - targetY) > 1f && i < maxIters) {
            transform.localEulerAngles = new Vector3(0, Mathf.LerpAngle(transform.localRotation.eulerAngles.y, targetY, Time.deltaTime * 8f), 0);
            i++;
            yield return new WaitForEndOfFrame();
        }

        if (anim != null && anim.HasState(0, Animator.StringToHash("attack"))) {
            anim.Play("attack");
        }

        yield return new WaitForSeconds(0.1f);
        SoundManager.instance.PlaySound(attackSound);
        yield return new WaitForSeconds(0.2f);
        
        target.Unit.TakeDamage(damage);
        
        GameManager.instance.moveInProgress = false;
        SetUnavailable();
    }

    public override void DestroySelf() {
        SoundManager.instance.PlaySound(deathSound);
        EventManager.instance.UnitDestroyed(this);
        Destroy(gameObject);
    }
}