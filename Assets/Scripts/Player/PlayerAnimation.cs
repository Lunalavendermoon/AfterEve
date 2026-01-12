using UnityEngine;
using Spine.Unity;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
public class PlayerAnimation : MonoBehaviour
{
    //spine skeleton
    private Transform parent;
    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState animationState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        animationState = skeletonAnimation.AnimationState;
        parent = transform.parent;
    }

    //dir should be -1 or 1
    public void SetY(int dir)
    {
        Vector3 scale = parent.localScale;
        scale.y = dir;
        parent.localScale = scale;
    }

    public void playRunAnimation()
    {
        animationState.SetAnimation(0, "animation", true);
    }

    //TODO: replace setup pose (empty animation) with idle animation
    public void playIdleAnimation()
    {
        animationState.SetEmptyAnimation(0, 0.2f);
    }
}
