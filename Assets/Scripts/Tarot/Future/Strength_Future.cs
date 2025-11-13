using UnityEngine;

public class Strength_Future : Future_TarotCard
{
    public const int effectGoal = 3;

    public Strength_Future(string s, int q) : base(s, q)
    {
        reward = new Strength_Reward(this);
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
}