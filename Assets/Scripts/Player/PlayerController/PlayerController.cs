using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public const string ScenePlayerRootName = "Player - Cyrus";

    private static PlayerController cachedScenePlayer;
    private static int cachedFrame = -1;

    public static PlayerController FindScenePlayer()
    {
        // Avoid repeated GameObject.Find calls by caching per-frame.
        // Refresh if cache is missing/destroyed.
        if (cachedFrame == Time.frameCount && cachedScenePlayer != null)
        {
            return cachedScenePlayer;
        }

        if (cachedScenePlayer != null)
        {
            cachedFrame = Time.frameCount;
            return cachedScenePlayer;
        }

        var root = GameObject.Find(ScenePlayerRootName);
        cachedScenePlayer = root != null ? root.GetComponentInChildren<PlayerController>(true) : null;
        cachedFrame = Time.frameCount;
        return cachedScenePlayer;
    }

    public static PlayerController Player => FindScenePlayer();

    // Kept for compatibility with existing code. Always resolves to the scene player.
    public static PlayerController instance => FindScenePlayer();

    private void OnEnable()
    {
        // Keep cache hot when the scene player is enabled.
        cachedScenePlayer = this;
        cachedFrame = Time.frameCount;
        EnablePlayerInput();
    }

    private void OnDisable()
    {
        if (cachedScenePlayer == this) cachedScenePlayer = null;
        DisablePlayerInput();
    }
    public HealthBarScript healthBar;
    public QuestUIScript questUIScript;

    private Rigidbody2D rb;

    // Small skin so we don't end up exactly touching and jittering
    private const float CastSkin = 0.01f;

    private readonly RaycastHit2D[] castResults = new RaycastHit2D[8];

    private void Awake()
    {
        playerInput = new PlayerInput();
        toggleDialogueLog = playerInput.FindAction("ToggleDialogueLog", true);

        ClearFutureSkills();
    }

    // user input system
    public PlayerInput playerInput;
    private InputAction toggleDialogueLog; // action is handled in DialogueHistoryLogUI
    bool inputEnabled;
    public float horizontalInput;
    public float verticalInput;

    public TMP_Text interactPrompt;

    // Player movement speed (exposed for designer control)
    public int speed;


    public void EnablePlayerInput()
    {
        playerInput.Enable();
        playerInput.Player.Enable();
        toggleDialogueLog.Disable();  
        inputEnabled = true;
    }

    public void DisablePlayerInput()
    {
        playerInput.Disable();
        playerInput.Player.Disable();
        toggleDialogueLog.Enable(); // dialogue log can only be toggled when dialogue is playing, which is when all other inputs are disabled
        inputEnabled = false;
    }

    // player attributes
    int coins = 0;
    public PlayerAttributes playerAttributes;

    // Authoritative runtime health (do not store in PlayerAttributes, since effects rebuild attributes)
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
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
    private bool inSpiritualVision;

    // future card skill
    public List<Future_Reward> futureSkills = new();
    public Queue<Future_Reward> futureSkillQueue = new();

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
    GameObject currentClone;

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

        // initialize health from attributes template once
        maxHealth = playerAttributes.maxHitPoints;
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SyncFromPlayer();
        }

        questUIScript.setQuestName("The Wheel of Fortune");
        questUIScript.setQuestDescription("Give Eve 8 million dollars because she wants it.");
        questUIScript.setQuestMaxValue(8);
        questUIScript.setQuestCurrentValue(7);

        // audio
        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps, transform.position);

        playerInput.Player.FutureSkill.performed += ctx => HandleFutureSkillInput((int)ctx.ReadValue<float>());

        interactPrompt.gameObject.SetActive(false);
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

        if (skillText != null)
        {
            skillText.text = BuildSkillDisplayString();
        }

        IPlayerState.speedCoefficient = speed;
        UpdateSound();
    }

    public void SyncMaxHealthToAttributes(bool fill = false)
    {
        if (playerAttributes == null) return;

        int attrMax = playerAttributes.maxHitPoints;
        if (attrMax <= 0) return;

        SetMaxHealth(attrMax, fill);
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
        s += "\nFuture skills:\n";
        foreach (var skill in futureSkills)
        {
            if (skill == null)
            {
                s += "- empty slot\n";
            }
            else
            {
                s += "- " + skill.ToString() + "\n";
            }
        }
        return s;
    }

    // TODO: FOR TESTING ONLY - delete in final!
    public void SetFutureSkill(string skill)
    {
        Future_Reward skillToAdd = skill switch
        {
            "chariot" => new Chariot_Reward(),
            "emperor" => new Emperor_Reward(),
            "empress" => new Empress_Reward(),
            "hierophant" => new Hierophant_Reward(),
            "highpriestess" => new HighPriestess_Reward(),
            "lovers" => new Lovers_Reward(),
            "magician" => new Magician_Reward(),
            "strength" => new Strength_Reward(),
            _ => null
        };
        TryAddFutureSkill(skillToAdd);
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

    public bool currentlyReloading = false;

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

    void HandleFutureSkillInput(int skillValue)
    {
        --skillValue;

        Future_Reward skill = futureSkills[skillValue];
        if (skill != null)
        {
            TarotManager.instance.futureSkillUI[skillValue].runTarotCooldownAnimation(skill.cooldown);
            skill.TriggerSkill();
        }
    }

    // Pass in "null" for skill to use the skill at the top of the skill queue
    // Returns true on success and false on failure
    public bool TryAddFutureSkill(Future_Reward skill = null)
    {
        bool dequeueOnSuccess = skill == null;
        bool enqueueOnFailure = skill != null;
        try
        {
            skill ??= futureSkillQueue.Peek(); // if skill is null, set its value
        }
        catch (InvalidOperationException)
        {
            // nothing to dequeue, i.e. trying to add a new skill failed
            return false;
        }

        for (int i = 0; i < futureSkills.Count; ++i)
        {
            Future_Reward target = futureSkills[i];
            if (target == null)
            {
                futureSkills[i] = skill;
                skill.SetSkillIndex(i);
                if (dequeueOnSuccess)
                {
                    futureSkillQueue.Dequeue();
                }
                TarotManager.instance.DisplayHand();
                return true;
            }
            else if (target.arcana == skill.arcana)
            {
                target.AddUses(skill.usesLeft);
                if (dequeueOnSuccess)
                {
                    futureSkillQueue.Dequeue();
                }
                TarotManager.instance.DisplayHand();
                return true;
            }
        }

        if (enqueueOnFailure)
        {
            futureSkillQueue.Enqueue(skill);
        }
        return false;
    }

    public void LoseFutureSkill(Future_Reward skill)
    {
        futureSkills[skill.GetSkillIndex()] = null;
        if (!TryAddFutureSkill()) // TryAddSkill() calls DisplayHand once on success, no need to call it again
        {
            TarotManager.instance.DisplayHand();
        }
    }

    public void GainFutureSkillSlot(int amount = 1)
    {
        if (StaticGameManager.futureSkillSlots + amount > StaticGameManager.maxSkillSlots)
        {
            amount = StaticGameManager.maxSkillSlots - StaticGameManager.futureSkillSlots;
        }
        StaticGameManager.futureSkillSlots += amount;

        while (amount > 0)
        {
            futureSkills.Add(null);
            --amount;
        }
        while (TryAddFutureSkill())
        {
        }
        // TODO: if empty future skill slots have specific visual, call TarotManager.instance.DisplayHand();
    }

    void ClearFutureSkills()
    {
        // Initialize all skill slots to be empty
        futureSkills = Enumerable.Repeat<Future_Reward>(null, StaticGameManager.futureSkillSlots).ToList();
        futureSkillQueue.Clear();
    }

    public virtual void TakeDamage(int amount, DamageInstance.DamageSource damageSource, DamageInstance.DamageType damageType)
    {
        // Debug.Log($"Incoming={amount} dmgTakenBonus={playerAttributes.damageTakenBonus} basicDef={playerAttributes.basicDefense}");
        AudioManager.instance.PlayOneShot(FMODEvents.instance.playerTakeDamage, this.transform.position);

        int originalAmt = amount;

        // damage reduction
        amount = playerAttributes.DamageCalculation(amount, DamageInstance.ToEnemyDamageType(damageType));
        // shield reduction (apply to already-reduced damage)
        amount = shieldManager.TakeShieldDamage(amount);
        amount = Mathf.Max(0, amount);

        // remaining damage is subtracted from player health
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        if (healthBar != null) healthBar.setCurrentHitPoints(currentHealth);
        //Debug.Log($"Player took {amount} damage, remaining health: {playerAttributes.currentHitPoints}, regular shield: {shieldManager.GetTotalShield(Shield.ShieldType.Regular)}, hitcount shield: {shieldManager.GetTotalShield(Shield.ShieldType.HitCount)}");
        OnDamageTaken?.Invoke(new DamageInstance(damageSource, damageType, originalAmt, amount));
        if (currentHealth <= 0)
        {
            Die(damageSource);
        }
    }

    public void Die(DamageInstance.DamageSource damageSource)
    {
        if (damageSource == DamageInstance.DamageSource.Enemy)
        {
            StaticGameManager.latestDeathCause = RepeatDeathRooms.DeathCauses.Enemy;
        }
        else if (damageSource == DamageInstance.DamageSource.ScriptedDeath)
        {
            StaticGameManager.latestDeathCause = RepeatDeathRooms.DeathCauses.ScriptedDeath;
        }
        else
        {
            StaticGameManager.latestDeathCause = RepeatDeathRooms.DeathCauses.Fallback;
        }
        
        ++StaticGameManager.deathCount;

        // Clear any active future skill on death.
        ClearFutureSkills();

        // Clear player tarot state on death (keep past cards).
        if (tarotManager != null)
        {
            tarotManager.ClearOnPlayerDeath();
        }
        else if (TarotManager.instance != null)
        {
            TarotManager.instance.ClearOnPlayerDeath();
        }

        StaticGameManager.LoadDeathScreen();
    }
    
    public void Heal(int amount)
    {
        currentHealth = Math.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"Player healed {amount}, current health: {currentHealth}");
        if (healthBar != null) healthBar.setCurrentHitPoints(currentHealth);
        // includes overflow healing in calculation :3
        OnHealed?.Invoke(amount);
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void SetMaxHealth(int value, bool fill = false)
    {
        maxHealth = Mathf.Max(1, value);
        if (fill) currentHealth = maxHealth;
        else currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.setMaxHitPoints(maxHealth);
            healthBar.setCurrentHitPoints(currentHealth);
        }
    }

    public void SetCurrentHealth(int value)
    {
        SetCurrentHealthInternal(value);
        if (healthBar != null) healthBar.setCurrentHitPoints(currentHealth);
    }

    private void SetCurrentHealthInternal(int value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
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
                //Debug.Log("stopping");
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
        if (currentClone != null)
        {
            Destroy(currentClone);
        }
        currentClone = clone;
        SetCloneDamage(a, b, c);
    }

    public void SetCloneDamage(float a, float b, float c)
    {
        currentClone.GetComponent<MimicPlayer>().SetDamage(a, b, c);
    }


    private void HandleInteractInput()
    {
        if (playerInput.Player.Interact.triggered)
        {
            currentInteractable?.TriggerInteraction();
        }
    }

    public bool IsInSpiritualVision()
    {
        return inSpiritualVision;
    }
}
