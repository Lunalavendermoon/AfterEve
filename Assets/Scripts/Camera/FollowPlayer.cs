using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform Player;
    public Vector3 offset;

    [SerializeField, Min(0f)] private float followSmoothTime = 0.09f;
    private const float SnapDistance = 3f;
    private Vector3 followVelocity;

    private void LateUpdate()
    {
        if (Player == null) return;

        Vector3 target = Player.position + offset;
        if (followSmoothTime <= 0f || (target - transform.position).sqrMagnitude > SnapDistance * SnapDistance)
        {
            transform.position = target;
            followVelocity = Vector3.zero;
            return;
        }

        if (Time.deltaTime <= 0f) return;

        transform.position = Vector3.SmoothDamp(
            transform.position, target, ref followVelocity, followSmoothTime);
    }
}
