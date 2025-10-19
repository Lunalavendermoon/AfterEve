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
    IPlayerState currentState;

    void Start()
    {
        currentState = new Player_Idle();
        currentState.EnterState(this);
    }

    void Update()
    {
        // handle input
        horizontalInput = playerInput.Player.Horizontal.ReadValue<float>();
        verticalInput = playerInput.Player.Vertical.ReadValue<float>();

        currentState.CheckState(this);
        currentState.UpdateState(this);
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }
}
