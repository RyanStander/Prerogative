using TMPro;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public CharacterManager characterManager;
    private Collider damageCollider;

    public float currentWeaponDamage=10;
    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damageable") || other.CompareTag("Enemy"))
        {
            //hanlde enemy/damageable being attack
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            CharacterManager targetCharacterManager = other.GetComponent<CharacterManager>();

            if (targetCharacterManager!=null)
            {
                if (targetCharacterManager.isParrying)
                {
                    if (characterManager != null)
                        //Check here if you are parryable
                        characterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Parried", true);
                    else
                        Debug.LogWarning("characterManager for damage collider was not set, please do so");
                    return;
                }
            }

            if (enemyStats != null)
            {
                enemyStats.TakeDamage(currentWeaponDamage);
            }            
        }
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            CharacterManager targetCharacterManager = other.GetComponent<CharacterManager>();

            if (targetCharacterManager != null)
            {
                if (targetCharacterManager.isParrying)
                {
                    if (characterManager != null)
                        //Check here if you are parryable
                        characterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Parried", true);
                    else
                        Debug.LogWarning("characterManager for damage collider was not set, please do so");
                    return;
                }
            }

            //handle player being attacked
            if (playerStats != null)
            {
                playerStats.TakeDamage(currentWeaponDamage);
            }
        }
    }
}
