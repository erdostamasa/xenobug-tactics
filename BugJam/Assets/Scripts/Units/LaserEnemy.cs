using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class LaserEnemy : EnemyUnit {
    [SerializeField] VolumetricLineBehavior line;
    [SerializeField] Transform attackPosition;
    

    public override void AttackAnimate(Unit target) {
        GameManager.instance.moveInProgress = true;
        //base.AttackAnimate(target);

        if (anim != null && anim.HasState(0, Animator.StringToHash("attack"))) {
            anim.Play("attack");
        }

        // line.EndPos = target.transform.position;
        StartCoroutine(LaserAnimation(target.currentTile));
    }

    IEnumerator LaserAnimation(Tile target) {
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


        //attack animation


        yield return new WaitForSeconds(0.05f);
        SoundManager.instance.PlaySound(attackSound);
        yield return new WaitForSeconds(0.05f);
        line.gameObject.SetActive(true);
        //line.StartPos = attackPosition.position;
        line.transform.forward = (target.unitPosition.transform.position - attackPosition.position).normalized;
        line.EndPos = new Vector3(0, 0, 5f);


        float time = 0;
        float duration = 0.3f;
        while (time < duration) {
            time += Time.deltaTime;
            line.transform.forward = (target.unitPosition.transform.position - attackPosition.position).normalized;
            yield return new WaitForEndOfFrame();
        }

        target.Unit.TakeDamage(damage);
        
        time = 0;
        duration = 0.2f;
        while (time < duration) {
            time += Time.deltaTime;
            line.transform.forward = (target.unitPosition.transform.position - attackPosition.position).normalized;
            yield return new WaitForEndOfFrame();
        }


        line.gameObject.SetActive(false);
        GameManager.instance.moveInProgress = false;
        SetUnavailable();
    }
}