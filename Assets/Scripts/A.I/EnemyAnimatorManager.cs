using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : AnimatorManager
{
    private EnemyManager enemyManager;
    private EnemyStats enemyStats;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyManager = GetComponentInParent<EnemyManager>();
        enemyStats = GetComponentInParent<EnemyStats>();
    }

    public override void TakeCriticalDamageAnimationEvent()
    {
        base.TakeCriticalDamageAnimationEvent();

        enemyStats.TakeDamage(enemyManager.pendingCriticalDamage, false);
        enemyManager.pendingCriticalDamage = 0;
    }

    private void OnAnimatorMove()
    {
        float delta = Time.deltaTime;
        enemyManager.enemyRigidBody.drag = 0;
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        enemyManager.enemyRigidBody.velocity = velocity;
    }

    public void EnableIsParrying()
    {
        enemyManager.isParrying = true;
    }
    public void DisableIsParrying()
    {
        enemyManager.isParrying = false;
    }

    public void EnableCanBeRiposted()
    {
        enemyManager.canBeRiposted = true;
    }

    public void DisableCanBeRiposted()
    {
        enemyManager.canBeRiposted = false;
    }

    public void EnableCombo()
    {
        anim.SetBool("canDoCombo", true);
    }

    public void DisableCombo()
    {
        anim.SetBool("canDoCombo", false);
    }

    public void EnableInvulnerability()
    {
        anim.SetBool("isInvulnerable", true);
    }

    public void DisableInvulnerability()
    {
        anim.SetBool("isInvulnerable", false);
    }
}
