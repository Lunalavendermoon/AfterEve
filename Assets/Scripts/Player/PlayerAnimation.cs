using UnityEngine;
using Spine.Unity;

[DefaultExecutionOrder(10)]
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

    [SerializeField] private Sprite aimingArmSprite;
    [SerializeField] private Sprite backAimingArmSprite;
    [SerializeField] private float aimingArmHeight = 24f;
    [SerializeField] private Vector2 muzzleInSprite = new Vector2(0.26f, 0.025f);
    [SerializeField] private Vector2 muzzleInBackSprite = new Vector2(0.74f, 0.025f);

    private static readonly string[] FrontArmSlots = { "UpperArm L", "LowerArm L", "Hand L" };
    private static readonly string[] BackArmSlots = { "Back_Cyrus_RARM", "Back_Cyrus_CoverRARM" };
    private Renderer bodyRenderer;
    private SpriteRenderer aimingArm;
    private Spine.Bone shoulder;
    private Spine.Slot[] armSlots;
    private Spine.Attachment[] originalAttachments;
    private bool usingBackArmArt;
    private float aimAngle;

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
        bodyRenderer = GetComponent<Renderer>();

        //initialize with front skeleton by default
        currentSkeletonData = frontSkeletonData;
        currentRunAnimationName = frontRunAnimationName;

        if (aimingArmSprite != null || backAimingArmSprite != null)
        {
            var arm = new GameObject("Aiming Arm");
            arm.layer = gameObject.layer;
            arm.transform.SetParent(transform, false);
            aimingArm = arm.AddComponent<SpriteRenderer>();
            aimingArm.enabled = false;
        }
    }

    private void OnEnable()
    {
        BindAimingArm();
        skeletonAnimation.UpdateComplete += HideAnimatedArm;
    }

    private void OnDisable()
    {
        skeletonAnimation.UpdateComplete -= HideAnimatedArm;
        if (aimingArm != null)
            aimingArm.enabled = false;

        if (armSlots == null)
            return;

        for (int i = 0; i < armSlots.Length; i++)
            if (armSlots[i] != null)
                armSlots[i].Attachment = originalAttachments[i];
    }

    void Update()
    {
        if (TryGetMouseWorldPosition(transform.position.z, out Vector3 mouse))
            UpdateSkeletonAndFlip(mouse - parent.position);
    }

    private void LateUpdate()
    {
        if (!TryGetArmShot(out _, out _))
            return;

        aimingArm.sortingLayerID = bodyRenderer.sortingLayerID;
        aimingArm.sortingOrder = bodyRenderer.sortingOrder + 1;
        aimingArm.enabled = true;
    }

    //dir should be -1 or 1
    public void SetY(int dir)
    {
        Vector3 scale = parent.localScale;
        scale.y = dir;
        parent.localScale = scale;
    }

    private void UpdateSkeletonAndFlip(Vector3 aimOffset)
    {
        UpdateSkeletonByVerticalMousePosition(aimOffset);
        UpdateFlipByHorizontalMousePosition(aimOffset);
    }

    private void UpdateSkeletonByVerticalMousePosition(Vector3 aimOffset)
    {
        bool aimUp = aimOffset.y > 0f;
        SkeletonDataAsset targetSkeleton = aimUp ? backSkeletonData : frontSkeletonData;
        string targetAnimationName = aimUp ? backRunAnimationName : frontRunAnimationName;

        // Only switch skeleton if it changed
        if (targetSkeleton != null && currentSkeletonData != targetSkeleton)
        {
            currentSkeletonData = targetSkeleton;
            currentRunAnimationName = targetAnimationName;
            skeletonAnimation.skeletonDataAsset = targetSkeleton;
            skeletonAnimation.Initialize(true);
            animationState = skeletonAnimation.AnimationState;
            BindAimingArm();

            if (PlayerController.instance != null && PlayerController.instance.IsMoving())
            {
                playRunAnimation();
            }
        }
    }

    private void UpdateFlipByHorizontalMousePosition(Vector3 aimOffset)
    {
        bool mouseOnRightSide = aimOffset.x >= 0f;

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

    private void BindAimingArm()
    {
        if (aimingArm == null)
            return;

        Spine.Skeleton skeleton = skeletonAnimation.Skeleton;
        bool usingBackRig = skeletonAnimation.skeletonDataAsset == backSkeletonData;
        usingBackArmArt = usingBackRig && backAimingArmSprite != null;
        Sprite sprite = usingBackArmArt ? backAimingArmSprite : aimingArmSprite;
        shoulder = sprite != null ? skeleton?.FindBone(usingBackRig ? "RUpArm" : "UpperArm L") : null;
        aimingArm.enabled = false;
        armSlots = null;
        if (shoulder == null)
            return;

        aimingArm.sprite = sprite;
        aimingArm.transform.localScale = Vector3.one * (aimingArmHeight / sprite.bounds.size.y);

        string[] slotNames = usingBackRig ? BackArmSlots : FrontArmSlots;
        armSlots = new Spine.Slot[slotNames.Length];
        originalAttachments = new Spine.Attachment[slotNames.Length];
        for (int i = 0; i < slotNames.Length; i++)
        {
            armSlots[i] = skeleton.FindSlot(slotNames[i]);
            originalAttachments[i] = armSlots[i]?.Attachment;
        }
        HideAnimatedArm(skeletonAnimation);
    }

    private void HideAnimatedArm(ISkeletonAnimation animated)
    {
        if (shoulder == null)
            return;

        foreach (Spine.Slot slot in armSlots)
            if (slot != null)
                slot.Attachment = null;
    }

    public bool TryGetArmShot(out Vector3 muzzle, out float angle)
    {
        muzzle = default;
        angle = default;
        if (!isActiveAndEnabled || aimingArm == null ||
            !TryGetMouseWorldPosition(transform.position.z, out Vector3 mouse))
            return false;

        UpdateSkeletonAndFlip(mouse - parent.position);
        if (shoulder == null)
            return false;

        Vector3 shoulderPosition = transform.TransformPoint(shoulder.WorldX, shoulder.WorldY, 0f);
        Vector2 direction = mouse - shoulderPosition;
        if (direction.sqrMagnitude > 0.000001f)
            aimAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Convert the muzzle from normalized image coordinates to an offset from the shoulder pivot.
        Sprite sprite = aimingArm.sprite;
        Vector2 muzzleInImage = usingBackArmArt ? muzzleInBackSprite : muzzleInSprite;
        Vector2 muzzleOffset = (Vector2.Scale(muzzleInImage, sprite.rect.size) -
            sprite.pivot) / sprite.pixelsPerUnit;
        aimingArm.flipX = usingBackArmArt ? !facingRight : facingRight;
        if (aimingArm.flipX)
            muzzleOffset.x = -muzzleOffset.x;

        float artAngle = Mathf.Atan2(muzzleOffset.y, muzzleOffset.x) * Mathf.Rad2Deg;
        aimingArm.transform.SetPositionAndRotation(shoulderPosition, Quaternion.Euler(0, 0, aimAngle - artAngle));
        muzzle = aimingArm.transform.TransformPoint(muzzleOffset);
        angle = aimAngle;
        return true;
    }

    public static bool TryGetMouseWorldPosition(float planeZ, out Vector3 position)
    {
        Camera camera = Camera.main;
        if (camera != null)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.forward, new Vector3(0f, 0f, planeZ));
            if (plane.Raycast(ray, out float distance))
            {
                position = ray.GetPoint(distance);
                return true;
            }
        }

        position = default;
        return false;
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
