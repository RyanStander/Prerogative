using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public Transform lockOnTransform;

    [Header("Combat Colliders")]
    public BoxCollider backstabBoxCollider;
    public BackstabCollider backstabCollider;
}
