using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EmotionCapsule : MonoBehaviour
{
    [SerializeField] private float shieldRadius = 2f;
    [SerializeField] private GameObject circleVisual;
    [SerializeField] private bool isActivated;
    public bool IsActivated => isActivated;
    public void ActivateFromSpiritualDamage()
    {
        isActivated = true;
        if (circleVisual != null)
            circleVisual.SetActive(true);
    }
    public bool IsPositionInsideShield(Vector3 worldPosition)
    {
        return isActivated && Vector3.Distance(transform.position, worldPosition) <= shieldRadius;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Projectile projectile = other.GetComponent<Projectile>();
        if (projectile == null)
            return;
        if (projectile.SpiritualDamage <= 0)
            return;
        ActivateFromSpiritualDamage();
    }
}
