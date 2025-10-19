using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class OldPlayerController : MonoBehaviour
{
    // make into singleton
    public static OldPlayerController instance;

    private void Awake()
    {
        if (instance == null) instance = this;

        playerInput = new PlayerInput();
    }

    public enum PlayerState
    {
        Idle,
        Move,
        Dash
    }

    // user input system
    private PlayerInput playerInput;
    private float horizontalInput;
    private float verticalInput;


    // player attributes
    public PlayerAttributes playerAttributes;

    // dashing mechanisms
    public float dashPower = 1.05f;
    public float dashDuration = .001f;
    public float dashStartTime;
    public Vector3 dashDirection;

    // state machine
    public PlayerState currentState;


    void OnEnable()
    {
        playerInput.Enable();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }


    void Start()
    {
        currentState = PlayerState.Idle;
    }

    void Update()
    {
        HandleInput();

        UpdateState();

        switch (currentState)
        {
            case PlayerState.Idle:
                // i don't know yet
                break;
            case PlayerState.Move:
                UpdateMove();
                break;
            case PlayerState.Dash:
                UpdateMove();
                UpdateDash();
                break;
        }

        if(playerInput.Player.Attack.IsPressed())
        {
            
        }
    }

    void HandleInput()
    {
        horizontalInput = playerInput.Player.Horizontal.ReadValue<float>();
        verticalInput = playerInput.Player.Vertical.ReadValue<float>();
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                if (horizontalInput != 0 || verticalInput != 0)
                {
                    currentState = PlayerState.Move;
                }
                break;
            case PlayerState.Move:
                if (horizontalInput == 0 && verticalInput == 0)
                {
                    currentState = PlayerState.Idle;
                }
                if (playerInput.Player.Dash.triggered) StartDash();
                break;
            case PlayerState.Dash:
                if (Time.time > dashStartTime + dashDuration)
                {
                    currentState = PlayerState.Idle;
                }
                break;
        }
    }

    void UpdateMove()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.x += horizontalInput * playerAttributes.speed * Time.deltaTime;
        currentPosition.y += verticalInput * playerAttributes.speed * Time.deltaTime;
        transform.position = currentPosition;
    }

    void StartDash()
    {
        dashStartTime = Time.time;
        dashDirection = new Vector3(horizontalInput, verticalInput, 0);
        currentState = PlayerState.Dash;
    }
    
    void UpdateDash()
    {
        float dashMultiplier = (dashDuration - (Time.time - dashStartTime)) / (dashDuration/2);
        transform.position += playerAttributes.speed * dashPower * dashMultiplier * Time.deltaTime * dashDirection;
    }
}
