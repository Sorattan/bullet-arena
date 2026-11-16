using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private EnemyAgent enemyPrefab;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField, Range(1, 10)] private int waveCount = 3;
    [SerializeField, Min(1)] private int enemiesInFirstWave = 3;
    [SerializeField, Min(0.1f)] private float spawnInterval = 1.4f;
    [SerializeField, Min(1f)] private float spawnRadius = 10f;
    [SerializeField, Min(0f)] private float waveBreakSeconds = 2f;

    private int currentWave;
    private int remainingEnemies;
    private int totalEnemiesThisWave;
    private int damageThisWave;
    private float waveStartedAt;
    private bool spawning;
    private bool finished;

    public void Configure(Transform playerTarget, EnemyAgent prefab, PlayerHealth health)
    {
        player = playerTarget;
        enemyPrefab = prefab;
        playerHealth = health;
    }

    private void Start()
    {
        if (player == null || enemyPrefab == null || playerHealth == null)
        {
            Debug.LogError("[Bullet Arena] WaveManager referansları eksik.", this);
            enabled = false;
            return;
        }
        playerHealth.Damaged += RecordDamage;
        playerHealth.Died += OnPlayerDied;
        StartCoroutine(BeginNextWave());
    }

    private void OnDestroy()
    {
        if (playerHealth == null) return;
        playerHealth.Damaged -= RecordDamage;
        playerHealth.Died -= OnPlayerDied;
    }

    private IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(currentWave == 0 ? 0.5f : waveBreakSeconds);
        if (finished) yield break;

        currentWave++;
        totalEnemiesThisWave = enemiesInFirstWave + currentWave - 1;
        remainingEnemies = 0;
        damageThisWave = 0;
        waveStartedAt = Time.time;
        spawning = true;

        for (int i = 0; i < totalEnemiesThisWave; i++)
        {
            if (finished) yield break;
            SpawnEnemy();
            if (i < totalEnemiesThisWave - 1)
                yield return new WaitForSeconds(spawnInterval);
        }
        spawning = false;
        CheckWaveCompleted();
    }

    private void SpawnEnemy()
    {
        Vector2 planar = Random.insideUnitCircle;
        if (planar.sqrMagnitude < 0.01f) planar = Vector2.right;
        planar.Normalize();
        Vector3 pos = player.position + new Vector3(planar.x, 0f, planar.y) * spawnRadius;
        pos.x = Mathf.Clamp(pos.x, -13f, 13f);
        pos.z = Mathf.Clamp(pos.z, -13f, 13f);
        pos.y = 1f;

        EnemyAgent enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        enemy.Initialize(player);
        enemy.Defeated += OnEnemyDefeated;
        remainingEnemies++;
    }

    private void OnEnemyDefeated(EnemyAgent enemy)
    {
        enemy.Defeated -= OnEnemyDefeated;
        remainingEnemies = Mathf.Max(remainingEnemies - 1, 0);
        CheckWaveCompleted();
    }

    private void RecordDamage(int amount)
    {
        if (currentWave > 0 && !finished) damageThisWave += amount;
    }

    private void CheckWaveCompleted()
    {
        if (finished || spawning || remainingEnemies > 0 || currentWave == 0) return;
        SaveCurrentWave(true);
        if (currentWave >= waveCount)
        {
            finished = true;
            Debug.Log("[Bullet Arena] Prototipteki tüm dalgalar tamamlandı.");
        }
        else
            StartCoroutine(BeginNextWave());
    }

    private void OnPlayerDied()
    {
        if (finished) return;
        finished = true;
        StopAllCoroutines();
        if (currentWave > 0) SaveCurrentWave(false);
        Debug.Log("[Bullet Arena] Oyuncu yenildi.");
    }

    private void SaveCurrentWave(bool success)
    {
        WaveTelemetry.Append(currentWave, totalEnemiesThisWave, spawnInterval,
            Time.time - waveStartedAt, damageThisWave, success);
    }

    private void OnGUI()
    {
        if (playerHealth == null) return;
        GUI.Box(new Rect(12, 12, 280, 95), "BULLET ARENA - PROTOTIP");
        GUI.Label(new Rect(24, 39, 260, 20),
            $"Can: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}   Dalga: {currentWave}/{waveCount}");
        GUI.Label(new Rect(24, 61, 260, 20), $"Kalan dusman: {remainingEnemies}");
        if (finished)
            GUI.Label(new Rect(24, 82, 260, 20), playerHealth.IsAlive ? "Prototip tamamlandi" : "Oyun bitti");
    }
}
