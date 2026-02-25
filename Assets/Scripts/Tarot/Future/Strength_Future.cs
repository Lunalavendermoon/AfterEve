using UnityEngine;

public class Strength_Future : Future_TarotCard
{
    public const int effectGoal = 3;

    public Strength_Future(int q) : base(q)
    {
        cardName = "Strength_Future";
        reward = new Strength_Reward(this);
        arcana = Arcana.Strength;
    }

    public override void ApplyCard(TarotManager tarotManager)
    {
        PlayerEffectManager.OnEffectAdded += OnEffectApplied;
    }

    protected override void RemoveListeners()
    {
        PlayerEffectManager.OnEffectAdded -= OnEffectApplied;
    }

    private void OnEffectApplied()
    {
        PlayerEffectManager effectManager = PlayerController.instance.gameObject.GetComponent<PlayerEffectManager>();
        if (effectManager.debuffs.Count >= effectGoal || effectManager.buffs.Count >= effectGoal)
        {
            CompleteQuest();
        }
    }

    public string temp()
    {
        return $"have total of {effectGoal} debuffs or buffs at the same time";
    }
}