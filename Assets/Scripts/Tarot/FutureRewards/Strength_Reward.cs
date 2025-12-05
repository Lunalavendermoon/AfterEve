using UnityEngine;

public class Strength_Reward : Future_Reward
{
    const float zoneDuration = 5f;
    const float maxZoneDistance = 5f;

    public Strength_Reward(Future_TarotCard card) : base(3, 5f, card)
    {
    }

    protected override void TriggerSkillBehavior()
    {
        Debug.Log("Triggered Strength skill");

        Vector3 playerPos = PlayerController.instance.gameObject.transform.position;

        Vector3 inp = Input.mousePosition;
        inp = Camera.main.ScreenToWorldPoint(inp);

        Vector3 displacement = inp - playerPos;
        displacement.z = 0f;

        Vector3 zonePos = Vector3.Normalize(displacement) * Mathf.Min(maxZoneDistance, displacement.magnitude) + playerPos;

        PlayerController.instance.SpawnFuturePrefab(FuturePrefabs.StrengthZone, zoneDuration, true, zonePos.x, zonePos.y, zonePos.z);
    }

    public override string GetName()
    {
        return "Strength Skill";
    }
}