using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EmotionCapsule : MonoBehaviour
{
    [SerializeField] private float shieldRadius = 2f;
    private bool isActivated;
    public bool IsActivated => isActivated;
    public void ActivateFromSpiritualDamage()
    {
        isActivated = true;
    }
    public bool IsPositionInsideShield(Vector3 worldPosition)
    {
        return isActivated && Vector3.Distance(transform.position, worldPosition) <= shieldRadius;
    }
}
