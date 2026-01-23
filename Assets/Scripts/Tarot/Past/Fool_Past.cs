public class Fool_Past : Past_TarotCard
{
    static float physicalDmgBonus = 0.7f;
    static int maxStackPerFoolCard = 7;
    int foolCardCount = 0;
    int stackCounter = 0;

    public Fool_Past(int q) : base(q)
    {
        cardName = "Fool_Past";
        arcana = Arcana.Fool;

        stackCounter = 0;
    }

    protected override void ApplyListeners()
    {
        // TODO
        // on obtain card (takes in card's arcana)
        // on enemy hit (takes in the enemy)
    }

    void OnObtainCard(Arcana arcana)
    {
        if (arcana == Arcana.Fool)
        {
            foolCardCount += 1;
        }

        if (stackCounter >= maxStackPerFoolCard * foolCardCount)
        {
            return;
        }

        ++stackCounter;
        // TODO apply physical dmg bonus
        // TODO: instead of having it be an effect, store the total dmg bonus in this card itself, and call OnEnemyHit
        // in THIS CARD class
        // whenever an enemy takes dmg to deal additional damage to it.
        // easier than adding 10 billion status effects/player attribute variables for each pastCard that gives dmg bonus
    }
}