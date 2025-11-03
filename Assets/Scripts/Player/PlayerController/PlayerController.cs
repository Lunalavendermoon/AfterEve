using System;
using UnityEditorInternal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // make into singleton
    public static PlayerController instance;

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

    //weapon
    private int currentBullets;
    private float lastReload;

    //spiritual vision
    private float currentSpiritualVision;
    private float spiritualVisionTimer;
    private Boolean inSpiritualVision;

    void Start()
    {
        currentState = new Player_Idle();
        currentState.EnterState(this);
        currentBullets = playerAttributes.Ammo;
        currentSpiritualVision = playerAttributes.totalSpiritualVision;
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
            HandleRotationInput();
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

    void HandleRotationInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 direction = hitPoint - transform.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
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
        }
        if (inSpiritualVision)
        {
            currentSpiritualVision -= (Time.time - spiritualVisionTimer);
            if (currentSpiritualVision <= 0)
            {
                currentSpiritualVision = 0;
                inSpiritualVision = false;
            }
        } else
        {
            if (!(currentSpiritualVision >= playerAttributes.totalSpiritualVision))
            {
                currentSpiritualVision += playerAttributes.spiritualVisionRegeneration * (Time.time - spiritualVisionTimer);
            } else
            {
                currentSpiritualVision = playerAttributes.totalSpiritualVision;
            }
        }

    }
}
