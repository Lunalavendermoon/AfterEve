using System.Collections.Generic;
using UnityEngine;

public class Chariot_Past : Past_TarotCard
{
    PlayerEffectManager effectManager;
    List<EffectInstance> ei = new();

    public const float damageBonus = 0.05f;
    public const int maxStacks = 10;

    public const float timerDur = 5f;
    float elapsedTime = 0f;
    float timeout = 0f;
    bool inCombat = false;

    public Chariot_Past(int q) : base(q)
    {
        cardName = "Chariot_Past";
        arcana = Arcana.Chariot;
    }

    protected override void ApplyListenersEffects()
    {
        effectManager = PlayerController.instance.gameObject.GetComponent<PlayerEffectManager>();

        PlayerController.OnDamageTaken += OnDamageTaken;
        EnemyBase.OnEnemyDamageTaken += OnDamageDealt;
    }

    public override void UpdateCard()
    {
        if (!inCombat || ei.Count >= maxStacks)
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
            ei.Add(effectManager.AddBuff(new ChariotPast_Effect()));
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

        for (int i = ei.Count; i >= 0; --i) {
            effectManager.RemoveEffect(ei[i]);
            ei.RemoveAt(i);
        }
    }

    protected override void GetLocalizedDesc()
    {
        base.GetLocalizedDesc();
        
        SetTableEntries("Chariot");

        SetDescriptionValues();
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