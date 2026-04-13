using UnityEngine;

public static class BossSummonUtility
{
    const int DefaultMaxAttempts = 80;
    public static void ConfigureAsBossSummon(StandardEnemyBase enemy)
    {
        if (enemy == null) return;
        enemy.chest = null;
        enemy.spawner = null;
        enemy.givesRewards = false;
    }
    public static StandardEnemyBase SpawnBossMinion(GameObject prefab, Vector3 worldPosition)
    {
        return SpawnBossMinion(prefab, worldPosition, Quaternion.identity);
    }
    public static StandardEnemyBase SpawnBossMinion(GameObject prefab, Vector3 worldPosition, Quaternion rotation)
    {
        if (prefab == null) return null;
        GameObject go = Object.Instantiate(prefab, worldPosition, rotation);
        StandardEnemyBase se = go.GetComponent<StandardEnemyBase>();
        ConfigureAsBossSummon(se);
        return se;
    }
    /// <summary>Uses entry.spawnPoint if set; otherwise random near boss inside room/map.</summary>
    public static StandardEnemyBase SpawnBossMinionFromEntry(
        Vector3 bossWorldPosition,
        float randomRadiusWhenSpawnPointMissing,
        EnemySpawnerScript.EnemyEntry entry)
    {
        if (entry.enemyPrefab == null) return null;
        float z = bossWorldPosition.z;
        Vector3 pos;
        if (entry.spawnPoint != null)
            pos = entry.spawnPoint.position;
        else if (!TryGetRandomSpawnPosition(bossWorldPosition, randomRadiusWhenSpawnPointMissing, z, out pos))
            pos = bossWorldPosition;
        return SpawnBossMinion(entry.enemyPrefab, pos);
    }
    public static bool TryGetRandomSpawnPosition(Vector3 bossWorldPosition, float radius, float zWorld, out Vector3 spawnPosition)
    {
        for (int attempt = 0; attempt < DefaultMaxAttempts; attempt++)
        {
            Vector2 offset = Random.insideUnitCircle * radius;
            Vector2 candidate = new Vector2(bossWorldPosition.x + offset.x, bossWorldPosition.y + offset.y);
            if (NarrativeRoomManager.instance != null &&
                NarrativeRoomManager.instance.IsPointInsideCurrentRoom(candidate, 0f))
            {
                spawnPosition = new Vector3(candidate.x, candidate.y, zWorld);
                return true;
            }
            if (GameManager.instance != null &&
                GameManager.instance.IsWorldPointInsideMap(candidate, 0f))
            {
                spawnPosition = new Vector3(candidate.x, candidate.y, zWorld);
                return true;
            }
        }
        spawnPosition = new Vector3(bossWorldPosition.x, bossWorldPosition.y, zWorld);
        return false;
    }
}