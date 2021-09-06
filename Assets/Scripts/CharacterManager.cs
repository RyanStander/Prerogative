using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public Transform lockOnTransform;

    [Header("Combat Colliders")]
    public BoxCollider backstabBoxCollider;
    public BackstabCollider backstabCollider;

    //Damage will be inflicted during an animation event
    //Used in backstab or riposte animations
    [Tooltip("The damage dealt during backstabs/counters")]public float pendingCriticalDamage;
}
