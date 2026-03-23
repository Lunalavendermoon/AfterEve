using UnityEngine;

public class BossProjectileScript : MonoBehaviour
{
    private GameObject target;
    private float moveSpeed;
    private float maxMoveSpeed;
    private float trajectoryMaxRelativeHeight;
    private float distanceToTargetToDestroyProjectile = 1f;


    [SerializeField] private AnimationCurve trajectoryAnimationCurve;
    [SerializeField] private AnimationCurve axisCorrectionAnimationCurve;
    [SerializeField] private AnimationCurve projectileSpeedAnimationCurve;


    private Vector3 trajectoryStartPoint;
    private Vector3 projectileMoveDir;
    private Vector3 trajectoryRange;


    private float nextYTrajectoryPosition;
    private float nextXTrajectoryPosition;
    private float nextPositionYCorrectionAbsolute;
    private float nextPositionXCorrectionAbsolute;

    private const int SpiritualDamageAmount = 30;
    private void Start()
    {
        trajectoryStartPoint = transform.position;
    }


    private void Update()
    {


        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        UpdateProjectilePosition();
        if (Vector3.Distance(transform.position, target.transform.position) < distanceToTargetToDestroyProjectile)
        {
            if (PlayerController.instance != null)
                PlayerController.instance.TakeDamage(SpiritualDamageAmount, DamageInstance.DamageSource.Enemy, DamageInstance.DamageType.Spiritual);
            Destroy(target);
            Destroy(gameObject);
        }
    }


    private void UpdateProjectilePosition()
    {
        if (target == null) return;
        trajectoryRange = target.transform.position - trajectoryStartPoint;
        float rangeX = Mathf.Abs(trajectoryRange.x);
        float rangeY = Mathf.Abs(trajectoryRange.y);
        if (rangeX < 0.001f && rangeY < 0.001f)
        {
            transform.position = target.transform.position;
            return;
        }
        if (rangeX >= rangeY)
        {
            if (trajectoryRange.x < 0) moveSpeed = -moveSpeed;
            UpdatePositionWithYCurve();
        }
        else
        {
            if (trajectoryRange.y < 0) moveSpeed = -moveSpeed;
            UpdatePositionWithXCurve();
        }

    }


    private void UpdatePositionWithXCurve()
    {

        if (Mathf.Abs(trajectoryRange.y) < 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Mathf.Abs(moveSpeed) * Time.deltaTime);
            return;
        }
        float nextPositionY = transform.position.y + moveSpeed * Time.deltaTime;
        float nextPositionYNormalized = (nextPositionY - trajectoryStartPoint.y) / trajectoryRange.y;


        float nextPositionXNormalized = trajectoryAnimationCurve.Evaluate(nextPositionYNormalized);
        nextXTrajectoryPosition = nextPositionXNormalized * trajectoryMaxRelativeHeight;


        float nextPositionXCorrectionNormalized = axisCorrectionAnimationCurve.Evaluate(nextPositionYNormalized);
        nextPositionXCorrectionAbsolute = nextPositionXCorrectionNormalized * trajectoryRange.x;


        if (trajectoryRange.x > 0 && trajectoryRange.y > 0)
        {
            nextXTrajectoryPosition = -nextXTrajectoryPosition;
        }


        if (trajectoryRange.x < 0 && trajectoryRange.y < 0)
        {
            nextXTrajectoryPosition = -nextXTrajectoryPosition;
        }




        float nextPositionX = trajectoryStartPoint.x + nextXTrajectoryPosition + nextPositionXCorrectionAbsolute;


        Vector3 newPosition = new Vector3(nextPositionX, nextPositionY, target.transform.position.z);


        CalculateNextProjectileSpeed(nextPositionYNormalized);
        projectileMoveDir = newPosition - transform.position;


        transform.position = newPosition;
    }


    private void UpdatePositionWithYCurve()
    {
        if (Mathf.Abs(trajectoryRange.x) < 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Mathf.Abs(moveSpeed) * Time.deltaTime);
            return;
        }

        float nextPositionX = transform.position.x + moveSpeed * Time.deltaTime;
        float nextPositionXNormalized = (nextPositionX - trajectoryStartPoint.x) / trajectoryRange.x;


        float nextPositionYNormalized = trajectoryAnimationCurve.Evaluate(nextPositionXNormalized);
        nextYTrajectoryPosition = nextPositionYNormalized * trajectoryMaxRelativeHeight;


        float nextPositionYCorrectionNormalized = axisCorrectionAnimationCurve.Evaluate(nextPositionXNormalized);
        nextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * trajectoryRange.y;


        float nextPositionY = trajectoryStartPoint.y + nextYTrajectoryPosition + nextPositionYCorrectionAbsolute;


        Vector3 newPosition = new Vector3(nextPositionX, nextPositionY, target.transform.position.z);


        CalculateNextProjectileSpeed(nextPositionXNormalized);
        projectileMoveDir = newPosition - transform.position;


        transform.position = newPosition;
    }


    private void CalculateNextProjectileSpeed(float nextPositionXNormalized)
    {
        float nextMoveSpeedNormalized = projectileSpeedAnimationCurve.Evaluate(nextPositionXNormalized);


        moveSpeed = nextMoveSpeedNormalized * maxMoveSpeed;
    }


    public void InitializeProjectile(GameObject target, float maxMoveSpeed, float trajectoryMaxHeight)
    {
        this.target = target;
        this.maxMoveSpeed = maxMoveSpeed;


        float xDistanceToTarget = target.transform.position.x - transform.position.x;
        this.trajectoryMaxRelativeHeight = Mathf.Abs(xDistanceToTarget) * trajectoryMaxHeight;


    }


    public void InitializeAnimationCurves(AnimationCurve trajectoryAnimationCurve, AnimationCurve axisCorrectionAnimationCurve, AnimationCurve projectileSpeedAnimationCurve)
    {
        this.trajectoryAnimationCurve = trajectoryAnimationCurve;
        this.axisCorrectionAnimationCurve = axisCorrectionAnimationCurve;
        this.projectileSpeedAnimationCurve = projectileSpeedAnimationCurve;
    }


    public Vector3 GetProjectileMoveDir()
    {
        return projectileMoveDir;
    }


    public float GetNextYTrajectoryPosition()
    {
        return nextYTrajectoryPosition;
    }


    public float GetNextPositionYCorrectionAbsolute()
    {
        return nextPositionYCorrectionAbsolute;
    }


    public float GetNextXTrajectoryPosition()
    {
        return nextXTrajectoryPosition;
    }


    public float GetNextPositionXCorrectionAbsolute()
    {
        return nextPositionXCorrectionAbsolute;
    }

}
