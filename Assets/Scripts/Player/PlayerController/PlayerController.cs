using System;
using UnityEditorInternal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // make into singleton
    public static PlayerController instance;
    public HealthBarScript healthBar;

    private void Awake()
    {
        if (instance == null) instance = this;

        playerInput = new PlayerInput();
    }

    // user input system
    public PlayerInput playerInput;
    public float horizontalInput;
    public float verticalInput;

    void OnEnable()
    {
        playerInput.Enable();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }

    // player attributes
    public PlayerAttributes playerAttributes;

    // state machine
    public IPlayerState currentState;
    public IRotationState currentRotationState;

    //weapon
    private int currentBullets;
    private float lastReload;

    //spiritual vision
    private float currentSpiritualVision;
    private float spiritualVisionTimer;
    private Boolean inSpiritualVision;

    // events
    public static event Action<DamageInstance> OnDamageTaken;
    public static event Action<int> OnHealed;
    public static event Action<int> OnShielded;
    public static event Action<bool> OnSpiritualVisionChange;

    void Start()
    {
        currentState = new Player_Idle();
        currentState.EnterState(this);
        currentRotationState = new RotationState_N();
        currentRotationState.EnterState(this);
        currentBullets = playerAttributes.Ammo;
        currentSpiritualVision = playerAttributes.totalSpiritualVision;
        healthBar.setMaxHealth(playerAttributes.maxHitPoints);
    }

    void Update()
    {
        if (playerAttributes.isParalyzed)
        {
            currentState = new Player_Idle();
        } else
        {
            // handle input
            horizontalInput = playerInput.Player.Horizontal.ReadValue<float>();
            verticalInput = playerInput.Player.Vertical.ReadValue<float>();

            currentState.CheckState(this);
            currentState.UpdateState(this);
            currentRotationState.UpdateState(this);
            HandleShootInput();
            HandleSpiritualVision();
        }
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public void ChangeRotationState(IRotationState newState)
    {
        currentRotationState = newState;
        currentRotationState.EnterState(this);
    }

    float GetRawMouseAngle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 direction = hitPoint - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return (angle + 360) % 360;
        }
        return -1f;
    }

    public void CheckRotationTransition()
    {
        float rawAngle = GetRawMouseAngle();
        if (rawAngle == -1f) return;

        float fixedAngleStep = 360f / 8f;
        int index = Mathf.RoundToInt(rawAngle / fixedAngleStep) % 8;

        IRotationState nextState = GetStateForIndex(index);

        if (nextState.GetType() != currentRotationState.GetType())
        {
            ChangeRotationState(nextState);
        }
    }

    IRotationState GetStateForIndex(int index)
    {
        switch (index)
        {
            case 0: return new RotationState_E();   
            case 1: return new RotationState_NE();  
            case 2: return new RotationState_N();   
            case 3: return new RotationState_NW();  
            case 4: return new RotationState_W();   
            case 5: return new RotationState_SW();  
            case 6: return new RotationState_S();   
            case 7: return new RotationState_SE();  
            default: return currentRotationState;
        }
    }

    void HandleShootInput()
    {
        if(currentBullets != 0)
        {
            if(playerInput.Player.Attack.triggered)
            {
                PlayerGun.Instance.Shoot();
                currentBullets--;
            }

        } else
        {
            Reload();
        }
    }

    Boolean currentlyReloading = false;

    void Reload()
    {
        if (!currentlyReloading)
        {
            currentlyReloading = true;
            lastReload = Time.time;
        }
        else
        {
            if(Time.time - lastReload >= playerAttributes.reloadSpeed)
            {
                currentlyReloading = false;
                currentBullets = playerAttributes.Ammo;
            }
        }
    }

    void HandleSpiritualVision()
    {
        spiritualVisionTimer = Time.time;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inSpiritualVision = true;
            OnSpiritualVisionChange?.Invoke(true);
        }
        if (inSpiritualVision)
        {
            currentSpiritualVision -= (Time.time - spiritualVisionTimer);
            if (currentSpiritualVision <= 0)
            {
                currentSpiritualVision = 0;
                inSpiritualVision = false;
                OnSpiritualVisionChange?.Invoke(false);
            }
        }
        else
        {
            if (!(currentSpiritualVision >= playerAttributes.totalSpiritualVision))
            {
                currentSpiritualVision += playerAttributes.spiritualVisionRegeneration * (Time.time - spiritualVisionTimer);
            }
            else
            {
                currentSpiritualVision = playerAttributes.totalSpiritualVision;
            }
        }

    }

    public virtual void TakeDamage(int amount, DamageInstance.DamageSource damageSource, DamageInstance.DamageType damageType)
    {
        // TODO factor in damage reduction as well
        playerAttributes.hitPoints -= amount;
        Debug.Log($"Player took {amount} damage, remaining health: {playerAttributes.hitPoints}");
        OnDamageTaken?.Invoke(new DamageInstance(damageSource, damageType, amount, amount));
        if (playerAttributes.hitPoints <= 0)
        {
            // TODO: player died
        }
    }
    
    public void Heal(int amount)
    {
        playerAttributes.hitPoints = Math.Clamp(playerAttributes.hitPoints + amount, 0, playerAttributes.maxHitPoints);
        // includes overflow healing in calculation :3
        OnHealed?.Invoke(amount);
    }
    
    public void GainShield(int amount)
    {
        playerAttributes.shield += amount;
        OnShielded?.Invoke(amount);
    }
}
