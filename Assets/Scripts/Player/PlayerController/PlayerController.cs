using FMOD.Studio;
using FMODUnity;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    // make into singleton
    public static PlayerController instance;
    public HealthBarScript healthBar;
    public QuestUIScript questUIScript;

    private Rigidbody2D rb;

    // Small skin so we don't end up exactly touching and jittering
    private const float CastSkin = 0.01f;

    private readonly RaycastHit2D[] castResults = new RaycastHit2D[8];

    private void Awake()
    {
        if (instance == null) instance = this;

        playerInput = new PlayerInput();
        toggleDialogueLog = playerInput.FindAction("ToggleDialogueLog", true);
    }

    // user input system
    public PlayerInput playerInput;
    private InputAction toggleDialogueLog; // action is handled in DialogueHistoryLogUI
    bool inputEnabled;
    public float horizontalInput;
    public float verticalInput;

    // Player movement speed (exposed for designer control)
    public int speed;

    void OnEnable()
    {
        EnablePlayerInput();
    }

    void OnDisable()
    {
        DisablePlayerInput();
    }

    public void EnablePlayerInput()
    {
        playerInput.Enable();
        toggleDialogueLog.Disable();  
        inputEnabled = true;
    }

    public void DisablePlayerInput()
    {
        playerInput.Disable();
        toggleDialogueLog.Enable(); // dialogue log can only be toggled when dialogue is playing, which is when all other inputs are disabled
        inputEnabled = false;
    }

    // player attributes
    public int health;
    int coins = 500; // TODO - set to 0 in final version, this is for testing only
    public PlayerAttributes playerAttributes;
    public PlayerFuturePrefab playerFuturePrefab;
    private bool magicianSkillActive = false;
    private float magicianSkillTimer;

    // state machine
    public IPlayerState currentState;

    //weapon
    private int currentAmmo;
    private float lastReload;
    private float fireRate;

    //spiritual vision
    private float currentSpiritualVision;
    private Boolean inSpiritualVision;

    // future card skill
    public Future_Reward futureSkill = null;

    // events
    public static event Action<DamageInstance> OnDamageTaken;
    public static event Action<int> OnHealed;
    public static event Action<int> OnShielded;
    public static event Action<bool> OnSpiritualVisionChange;
    public static event Action<IPlayerState> OnPlayerStateChange;
    public static event Action OnCoinsChange;
    public static event Action OnCoinsDecrease;
    public static event Action<int> OnCoinsSpentAtShop;

    // FOR TESTING ONLY!
    public TMP_Text skillText;

    //audio
    private EventInstance playerFootsteps;

    //animations
    public PlayerAnimation playerAnimation;

    //clone
    public GameObject clonePrefab;

    // interactions
    public InteractableEntity currentInteractable;

    public TarotManager tarotManager;
    public ShieldManager shieldManager;

    //post processing
    public Volume volume;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("PlayerController requires a Rigidbody2D on the same GameObject.");
        }

        // movement state machine
        currentState = new Player_Idle();
        currentState.EnterState(this);
        OnPlayerStateChange?.Invoke(currentState);

        // weapon
        currentAmmo = playerAttributes.Ammo;
        AttackUI.Instance.initializeAmmoUI();

        currentSpiritualVision = playerAttributes.totalSpiritualVision;
        healthBar.setMaxHitPoints(playerAttributes.maxHitPoints);
        healthBar.setCurrentHitPoints(playerAttributes.maxHitPoints);
        health = playerAttributes.maxHitPoints;

        questUIScript.setQuestName("The Wheel of Fortune");
        questUIScript.setQuestDescription("Give Eve 8 million dollars because she wants it.");
        questUIScript.setQuestMaxValue(8);
        questUIScript.setQuestCurrentValue(7);

        // audio
        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps, this.transform.position);
    }

    void Update()
    {
        // decrement timer for active skills (if any)
        if (magicianSkillActive)
        {
            magicianSkillTimer -= Time.deltaTime;
            if (magicianSkillTimer <= 0f)
            {
                SetMagicianSkill(false);
            }
        }

        HandleInteractInput();

        if (playerAttributes.isParalyzed)
        {
            if (currentState is not Player_Idle)
            {
                ChangeState(new Player_Idle());
            }

            return;
        }

        // Read input in Update (recommended)
        horizontalInput = playerInput.Player.Horizontal.ReadValue<float>();
        verticalInput = playerInput.Player.Vertical.ReadValue<float>();

        // State checks still in Update
        currentState.CheckState(this);
        currentState.UpdateState(this);

        HandleShootInput();
        HandleSpiritualVision();
        HandleFutureSkillInput();

        if (skillText != null)
        {
            skillText.text = BuildSkillDisplayString();
        }

        IPlayerState.speedCoefficient = speed;
        UpdateSound();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateClone(1, 1, 1);
        }
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdateState(this);
    }

    /// <summary>
    /// Moves the player with collision using Rigidbody2D.Cast and MovePosition (prevents jitter and tunneling).
    /// </summary>
    public void TryMove(Vector2 delta)
    {
        if (rb == null) return;

        if (delta == Vector2.zero) return;

        float distance = delta.magnitude;
        Vector2 direction = delta / distance;

        int hitCount = rb.Cast(direction, castResults, distance + CastSkin);
        if (hitCount > 0)
        {
            float minDistance = distance;

            for (int i = 0; i < hitCount; i++)
            {
                // Ignore triggers
                if (castResults[i].collider != null && castResults[i].collider.isTrigger) continue;

                // Cast distance includes skin; we want to stop just before the wall
                float d = castResults[i].distance - CastSkin;
                if (d < minDistance)
                {
                    minDistance = Mathf.Max(0f, d);
                }
            }

            delta = direction * minDistance;
        }

        rb.MovePosition(rb.position + delta);
    }

    string BuildSkillDisplayString()
    {
        string s = "Coins: " + coins;
        s += "\nFuture skill: ";
        if (futureSkill == null)
        {
            s += "none";
            return s;
        }
        else
        {
            s += futureSkill.GetName();
        }
        s += "\n";
        if (futureSkill.IsOnCooldown())
        {
            s += "Cooldown: " + (int)(futureSkill.cooldown - Time.time + futureSkill.lastUseTime);
        } else
        {
            s += "Press E to use skill!";
        }
        s += "\nRemaining skill uses: " + futureSkill.usesLeft;
        return s;
    }

    // TODO: FOR TESTING ONLY - delete in final!
    public void SetFutureSkill(string skill)
    {
        switch (skill)
        {
            case "chariot":
                futureSkill = new Chariot_Reward(null);
                return;
            case "emperor":
                futureSkill = new Emperor_Reward(null);
                return;
            case "empress":
                futureSkill = new Empress_Reward(null);
                return;
            case "hierophant":
                futureSkill = new Hierophant_Reward(null);
                return;
            case "highpriestess":
                futureSkill = new HighPriestess_Reward(null);
                return;
            case "lovers":
                futureSkill = new Lovers_Reward(null);
                return;
            case "magician":
                futureSkill = new Magician_Reward(null);
                return;
            case "strength":
                futureSkill = new Strength_Reward(null);
                return;
        }
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
        OnPlayerStateChange?.Invoke(newState);
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

    void HandleShootInput()
    {
        if(currentAmmo != 0 || magicianSkillActive)
        {
            if (playerInput.Player.Attack.triggered)
            {
                bool shotFired;
                if (magicianSkillActive)
                {
                    shotFired = PlayerGun.Instance.ShootMagicianCoin();
                    ChangeCoins(-Magician_Reward.coinsPerShot);
                }
                else
                {
                    shotFired = PlayerGun.Instance.Shoot();
                }

                // Only decrease ammo and play gunshot sfx if shot was fired (not in cooldown)
                if (shotFired)
                {
                    if (!magicianSkillActive)
                    {
                        AttackUI.Instance.greyNextAmmo(currentAmmo);
                        currentAmmo--;
                    }
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.gunshot, this.transform.position);
                }
            }

        } else
        {
            //Debug.Log("Out of ammo. Reloading");
            Reload();
        }
        //Debug.Log("Remaining ammo: " + currentAmmo);
    }

    public void SetMagicianSkill(bool activated)
    {
        magicianSkillActive = activated;
        if (magicianSkillActive)
        {
            magicianSkillTimer = Magician_Reward.skillDuration;
        }
    }

    public bool IsMagicianSkillActive()
    {
        return magicianSkillActive;
    }

    Boolean currentlyReloading = false;

    void Reload()
    {
        AttackUI.Instance.runAmmoReloadAnimation();
        if (!currentlyReloading)
        {
            currentlyReloading = true;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.reload, this.transform.position);
            lastReload = Time.time;
        }
        else
        {
            if(Time.time - lastReload >= playerAttributes.reloadSpeed)
            {
                currentlyReloading = false;
                AttackUI.Instance.resetAmmoUI();
                currentAmmo = playerAttributes.Ammo;
            }
        }
    }

    void HandleSpiritualVision()
    {
        if (playerInput.Player.SpritualVision.IsPressed() && !inSpiritualVision)
        {
            inSpiritualVision = true;
            OnSpiritualVisionChange?.Invoke(true);
            if(volume.profile.TryGet(out WhiteBalance whiteBalance))
            {
                whiteBalance.temperature.value = -75f;
            }
        }
        if (inSpiritualVision)
        {
            currentSpiritualVision -= Time.deltaTime;
            if (currentSpiritualVision <= 0)
            {
                currentSpiritualVision = 0;
                inSpiritualVision = false;
                OnSpiritualVisionChange?.Invoke(false);
                if (volume.profile.TryGet(out WhiteBalance whiteBalance))
                {
                    whiteBalance.temperature.value = 0f;
                }
            }
        }
        else
        {
            if (!(currentSpiritualVision >= playerAttributes.totalSpiritualVision))
            {
                currentSpiritualVision += playerAttributes.spiritualVisionRegeneration * Time.deltaTime;
            }
            else
            {
                currentSpiritualVision = playerAttributes.totalSpiritualVision;
            }
        }

    }

    void HandleFutureSkillInput()
    {
        if (playerInput.Player.Skill.triggered)
        {
            futureSkill?.TriggerSkill();
        }
    }

    public virtual void TakeDamage(int amount, DamageInstance.DamageSource damageSource, DamageInstance.DamageType damageType)
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.playerTakeDamage, this.transform.position);

        int originalAmt = amount;

        // damage reduction
        amount = playerAttributes.DamageCalculation(amount, DamageInstance.ToEnemyDamageType(damageType));
        // shield reduction
        amount = shieldManager.TakeShieldDamage(playerAttributes.DamageCalculation(amount, DamageInstance.ToEnemyDamageType(damageType)));

        // remaining damage is subtracted from player health
        health -= amount;
        healthBar.setCurrentHitPoints(health);
        Debug.Log($"Player took {amount} damage, remaining health: {health}, regular shield: {shieldManager.GetTotalShield(Shield.ShieldType.Regular)}, hitcount shield: {shieldManager.GetTotalShield(Shield.ShieldType.HitCount)}");
        OnDamageTaken?.Invoke(new DamageInstance(damageSource, damageType, originalAmt, amount));
        if (health <= 0)
        {
            // TODO: player died
            Debug.Log("Player health reached 0");
        }
    }
    
    public void Heal(int amount)
    {
        health = Math.Clamp(health + amount, 0, playerAttributes.maxHitPoints);
        Debug.Log($"Player healed {amount}, current health: {health}");
        healthBar.setCurrentHitPoints(health);
        // includes overflow healing in calculation :3
        OnHealed?.Invoke(amount);
    }

    public int GetHealth()
    {
        return health;
    }
    
    public void GainRegularShield(int amount, float duration = -1)
    {
        if (duration <= 0)
        {
            shieldManager.GainRegularShield(amount);
        }
        else
        {
            shieldManager.AddShield(new Regular_Shield(amount, duration));
        }
        OnShielded?.Invoke(amount);
    }

    public void GainHitCountShield(int amount, float duration)
    {
        shieldManager.AddShield(new HitCount_Shield(amount, duration));
    }

    public void ChangeCoins(int amount, bool fromShop = false)
    {
        coins += amount;
        OnCoinsChange?.Invoke();
        if (amount < 0)
        {
            OnCoinsDecrease?.Invoke();
            if (fromShop)
            {
                OnCoinsSpentAtShop?.Invoke(-amount);
            }
        }
    }

    public int GetCoins()
    {
        return coins;
    }

    public void SpawnFuturePrefab(Future_Reward.FuturePrefabs targetPrefab, float duration,
                                    bool hasCustomPos = false, float x = 0f, float y = 0f, float z = 0f)
    {
        GameObject inst = null;

        Vector3 position;
        if (hasCustomPos)
        {
            position = new Vector3(x, y, z);
        }
        else
        {
            position = transform.position;
        }

        switch (targetPrefab)
        {
            case Future_Reward.FuturePrefabs.HighPriestessZone:
                inst = Instantiate(playerFuturePrefab.HighPriestessZone, position, Quaternion.identity);
                break;
            case Future_Reward.FuturePrefabs.EmpressPulse:
                inst = Instantiate(playerFuturePrefab.EmpressPulse, transform);
                break;
            case Future_Reward.FuturePrefabs.StrengthZone:
                inst = Instantiate(playerFuturePrefab.StrengthZone, position, Quaternion.identity);
                break;
            case Future_Reward.FuturePrefabs.LoverSummon:
                inst = Instantiate(playerFuturePrefab.LoverSummon, position, Quaternion.identity);
                break;
        }
        if (duration > 0f && inst != null)
        {
            Destroy(inst, duration);
        }
    }

    private void UpdateSound()
    {
        if (IsMoving())
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            playerFootsteps.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
            }
        } else
        {
            playerFootsteps.getPlaybackState(out PLAYBACK_STATE playbackState);
            if (playbackState != PLAYBACK_STATE.STOPPING && playbackState != PLAYBACK_STATE.STOPPED)
            {
                Debug.Log("stopping");
                playerFootsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
    }

    public bool IsMoving()
    {
        float horizontalValue = playerInput.Player.Horizontal.ReadValue<float>();
        float verticalValue = playerInput.Player.Vertical.ReadValue<float>();
        return Mathf.Abs(horizontalValue) > 0.01f || Mathf.Abs(verticalValue) > 0.01f;
    }

    public void CreateClone(float a, float b, float c) {
        GameObject clone = Instantiate(clonePrefab, new Vector3(transform.position.x+0.5f, transform.position.y, transform.position.z), Quaternion.identity);
        clone.GetComponent<MimicPlayer>().SetDamage(a, b, c);
    }


    private void HandleInteractInput()
    {
        if (playerInput.Player.Interact.triggered && currentInteractable != null)
        {
            currentInteractable.TriggerInteraction();
        }
    }

    public bool IsInSpiritualVision()
    {
        return inSpiritualVision;
    }
}
