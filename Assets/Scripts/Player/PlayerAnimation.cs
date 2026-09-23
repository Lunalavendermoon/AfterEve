using UnityEngine;
using Spine.Unity;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
public class PlayerAnimation : MonoBehaviour
{
    //spine skeleton
    private Transform parent;
    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState animationState;

    //skeleton data assets
    [SerializeField] private SkeletonDataAsset backSkeletonData;
    [SerializeField] private SkeletonDataAsset frontSkeletonData;

    //animation names for each skeleton
    [SerializeField] private string backRunAnimationName = "Walk";
    [SerializeField] private string frontRunAnimationName = "animation";

    //current skeleton tracking
    private SkeletonDataAsset currentSkeletonData;
    private string currentRunAnimationName;
    private bool facingRight = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        animationState = skeletonAnimation.AnimationState;
        parent = transform.parent;

        //initialize with front skeleton by default
        currentSkeletonData = frontSkeletonData;
        currentRunAnimationName = frontRunAnimationName;
    }

    void Update()
    {
        UpdateSkeletonAndFlip();
    }

    //dir should be -1 or 1
    public void SetY(int dir)
    {
        Vector3 scale = parent.localScale;
        scale.y = dir;
        parent.localScale = scale;
    }

    private void UpdateSkeletonAndFlip()
    {
        // Get mouse position in screen space
        Vector3 mouseScreenPos = Input.mousePosition;

        // Update skeleton based on vertical mouse position
        UpdateSkeletonByVerticalMousePosition(mouseScreenPos);

        // Update flip based on horizontal mouse position
        UpdateFlipByHorizontalMousePosition(mouseScreenPos);
    }

    private void UpdateSkeletonByVerticalMousePosition(Vector3 mouseScreenPos)
    {
        // Determine if mouse is in top half (back skeleton) or bottom half (front skeleton)
        bool mouseInTopHalf = mouseScreenPos.y > Screen.height / 2f;
        SkeletonDataAsset targetSkeleton = mouseInTopHalf ? backSkeletonData : frontSkeletonData;
        string targetAnimationName = mouseInTopHalf ? backRunAnimationName : frontRunAnimationName;

        // Only switch skeleton if it changed
        if (currentSkeletonData != targetSkeleton)
        {
            currentSkeletonData = targetSkeleton;
            currentRunAnimationName = targetAnimationName;
            if (targetSkeleton != null)
            {
                skeletonAnimation.skeletonDataAsset = targetSkeleton;
                skeletonAnimation.Initialize(true);

                // Restart animation if player is moving
                if (PlayerController.instance != null && PlayerController.instance.IsMoving())
                {
                    playRunAnimation();
                }
            }
        }
    }

    private void UpdateFlipByHorizontalMousePosition(Vector3 mouseScreenPos)
    {
        // Determine if mouse is on right side (face right) or left side (face left)
        bool mouseOnRightSide = mouseScreenPos.x > Screen.width / 2f;

        // Only update if facing direction changed
        if ((mouseOnRightSide && !facingRight) || (!mouseOnRightSide && facingRight))
        {
            facingRight = mouseOnRightSide;
            FlipSpineChild();
        }
    }

    private void FlipSpineChild()
    {
        // Flip the Spine - Cyrus child by rotating around Y axis
        // 0 rotation = facing right, 180 rotation = facing left
        float rotationY = facingRight ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    public void playRunAnimation()
    {
        animationState = skeletonAnimation.AnimationState;
        if (animationState != null)
        {
            animationState.SetAnimation(0, currentRunAnimationName, true);
        }
    }

    //TODO: replace setup pose (empty animation) with idle animation
    public void playIdleAnimation()
    {
        if (animationState != null)
        {
            animationState.SetEmptyAnimation(0, 0.2f);
        }
    }
}
