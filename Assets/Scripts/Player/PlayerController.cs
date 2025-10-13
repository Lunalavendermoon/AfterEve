using System.ComponentModel;
using Unity.VisualScripting;
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
    public float speed = 5f;

    // dashing mechanisms
    public float dashPower = 2f;
    public float dashDuration = .001f;
    public float dashStartTime;
    public Vector3 dashDirection;

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
                Vector3 currentPosition = transform.position;
                currentPosition.x += horizontalInput * speed * Time.deltaTime;
                currentPosition.y += verticalInput * speed * Time.deltaTime;
                transform.position = currentPosition;
                break;
            case PlayerState.Dash:
                float dashMultiplier = (dashDuration - (Time.time - dashStartTime)) / (dashDuration/2);
                transform.position += speed * dashPower * dashMultiplier * Time.deltaTime * dashDirection;
                break;
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
                if (playerInput.Player.Dash.triggered)
                {
                    dashStartTime = Time.time;
                    dashDirection = new Vector3(horizontalInput, verticalInput, 0);
                    currentState = PlayerState.Dash;
                }
                break;
            case PlayerState.Dash:
                if (Time.time > dashStartTime + dashDuration)
                {
                    currentState = PlayerState.Idle;
                }
                break;
        }
    }
}
