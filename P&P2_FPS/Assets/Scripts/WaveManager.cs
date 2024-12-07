using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private GameObject enemyPrefab; // Prefab of the enemy to spawn
    [SerializeField] private Transform[] spawnPoints; // Array of spawn points
    [SerializeField] private int initialEnemies = 5; // Number of enemies in the first wave
    [SerializeField] private float spawnDelay = 1.0f; // Delay between enemy spawns
    [SerializeField] private float timeBetweenWaves = 5.0f; // Time delay between waves

    [Header("Wave Tracking")]
    private int currentWave = 0; // Current wave number
    private int enemiesRemaining = 0; // Remaining enemies in the current wave

    [Header("References")]
    [SerializeField] private GameManager gameManager; // Reference to the GameManager
    private bool isWaveActive = false; // Tracks if a wave is currently active

    public int GetCurrentWave()
    {
        return currentWave;
    }
    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance; // Assign GameManager instance if not set
        }

        StartNextWave();
    }

    public void StartNextWave()
    {
        if (isWaveActive) return;

        currentWave++;
        enemiesRemaining = initialEnemies + (currentWave - 1) * 2; // Scale enemies per wave
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        isWaveActive = true;

        Debug.Log($"Starting Wave {currentWave} with {enemiesRemaining} enemies.");

        for (int i = 0; i < enemiesRemaining; i++)
        {
            // Select a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Spawn the enemy
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // Wait before spawning the next enemy
            yield return new WaitForSeconds(spawnDelay);
        }

        isWaveActive = false; // Mark wave spawning complete
    }

    public void EnemyDefeated()
    {
        enemiesRemaining--;
        Debug.Log($"Enemy defeated! {enemiesRemaining} remaining in Wave {currentWave}.");

        if (enemiesRemaining <= 0)
        {
            Debug.Log($"Wave {currentWave} completed!");
            Invoke(nameof(StartNextWave), timeBetweenWaves); // Start the next wave after a delay
        }
    }
}

