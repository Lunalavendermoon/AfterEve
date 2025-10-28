using System;
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

    //for weapon ammo system
    private int currentBullets;
    private float lastReload;
    public float reloadTime;

    void Start()
    {
        currentState = new Player_Idle();
        currentState.EnterState(this);
        currentBullets = playerAttributes.Ammo;
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
            if(Time.time - lastReload >= reloadTime)
            {
                currentlyReloading = false;
                currentBullets = playerAttributes.Ammo;
            }
        }
    }
}
