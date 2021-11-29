using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class LaserEnemy : Unit {
    [SerializeField] VolumetricLineBehavior line;
    [SerializeField] Transform attackPosition;

    public override void AttackAnimate(Unit target) {
        base.AttackAnimate(target);

        
        // line.EndPos = target.transform.position;
        StartCoroutine(LaserAnimation(target.currentTile));
    }

    IEnumerator LaserAnimation(Tile target) {
        yield return new WaitForSeconds(0.3f);
        
        line.gameObject.SetActive(true);
        //line.StartPos = attackPosition.position;
        line.transform.forward = (target.unitPosition.transform.position - attackPosition.position).normalized;
        line.EndPos = new Vector3(0, 0, 5f);
        
        
        float time = 0;
        float duration = 0.5f;
        while (time < duration) {
            time += Time.deltaTime;
            line.transform.forward = (target.unitPosition.transform.position - attackPosition.position).normalized;
            yield return new WaitForEndOfFrame();
        }

        line.gameObject.SetActive(false);
    }
}