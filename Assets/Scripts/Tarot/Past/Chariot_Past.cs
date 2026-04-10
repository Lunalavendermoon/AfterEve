using System.Collections.Generic;
using UnityEngine;

public class Chariot_Past : Past_TarotCard
{
    PlayerEffectManager effectManager;

    public const float damageBonus = 0.05f;
    public const int maxStacks = 10;

    public const float timerDur = 5f;
    float elapsedTime = 0f;
    float timeout = 0f;
    bool inCombat = false;

    public Chariot_Past() : base()
    {
        cardName = "Chariot_Past";
        arcana = Arcana.Chariot;

        GetLocalizedDesc();
    }

    protected override void ApplyListenersEffects(bool muted = false)
    {
        effectManager = PlayerController.instance.gameObject.GetComponent<PlayerEffectManager>();

        PlayerController.OnDamageTaken += OnDamageTaken;
        EnemyBase.OnEnemyDamageTaken += OnDamageDealt;
    }

    protected override void RemoveListeners(bool muted = false)
    {
        PlayerController.OnDamageTaken -= OnDamageTaken;
        EnemyBase.OnEnemyDamageTaken -= OnDamageDealt;
    }

    public override void UpdateCard()
    {
        if (!inCombat || effectInstances.Count >= maxStacks)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        timeout -= Time.deltaTime;

        // Debug.Log($"took elapsed time: {elapsedTime} timeout: {timeout}");

        if (timeout <= 0f)
        {
            EndCombat();
            return;
        }

        if (elapsedTime >= timerDur)
        {
            effectInstances.Add(effectManager.AddBuff(new ChariotPast_Effect()));
            elapsedTime -= timerDur;
        }
    }

    void OnDamageTaken(DamageInstance _)
    {
        BeginCombat();
    }

    void OnDamageDealt(DamageInstance _a, EnemyBase _b)
    {
        BeginCombat();
    }

    void BeginCombat()
    {
        if (!inCombat)
        {
            elapsedTime = 0f;
        }

        inCombat = true;
        timeout = timerDur;
    }

    void EndCombat()
    {
        inCombat = false;

        for (int i = effectInstances.Count; i >= 0; --i) {
            effectManager.RemoveEffect(effectInstances[i]);
            effectInstances.RemoveAt(i);
        }
    }

    protected override void SetDescriptionValues()
    {
        desc.Arguments = new object[]
        {
            Rnd(timerDur),
            FormatPercentage(damageBonus),
            FormatPercentage(maxStacks * damageBonus)
        };
    }
}