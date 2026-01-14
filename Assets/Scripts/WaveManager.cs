using UnityEngine;
using System.Collections;
using TMPro; // For TextMeshPro

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public GameObject enemyPrefab;
    public int startingEnemies = 3;
    public int enemiesPerWave = 2; // How many more enemies each wave
    public float spawnRadius = 10f; // How far from player to spawn
    public float timeBetweenWaves = 3f; // Delay before next wave starts
    
    [Header("References")]
    public Transform player;
    public TextMeshProUGUI waveText; // Reference to UI text
    public TextMeshProUGUI killCountText; // Reference to UI text
    private int currentWave = 0;
    private int enemiesAlive = 0;
    private int enemiesToSpawn = 0;
    private bool waveInProgress = false;

    private int totalKills = 0;
    
    [Header("Extraction Settings")]
    public GameObject extractionPointPrefab;
    public int waveToSpawnExtraction = 3; // Spawn after wave 3

    private GameObject spawnedExtractionPoint;
    private bool extractionActive = false;
    void Start()
    {
        // Start first wave
        StartNextWave();
        UpdateKillCountUI();
    }
    
    void Update()
    {
        // Check if wave is complete (but not during extraction)
        if (waveInProgress && enemiesAlive <= 0 && !extractionActive)
        {
            // Wave finished! Start next one after delay
            waveInProgress = false;
            Invoke("StartNextWave", timeBetweenWaves);
        }
    }
    
    void StartNextWave()
    {
        currentWave++;
        waveInProgress = true;
        
        enemiesToSpawn = startingEnemies + (currentWave - 1) * enemiesPerWave;
        
        UpdateWaveUI();
        
        Debug.Log("Wave " + currentWave + " started! Enemies: " + enemiesToSpawn);
        
        // Spawn extraction point after certain wave
        if (currentWave == waveToSpawnExtraction && spawnedExtractionPoint == null)
        {
            SpawnExtractionPoint();
        }
        
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
        }
        
        enemiesAlive = enemiesToSpawn;
    }
    
    void SpawnEnemy()
    {
        // Random position around the player
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = player.position + (Vector3)(randomDirection * spawnRadius);
        
        // Spawn the enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        // Subscribe to enemy death
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.OnDeath += HandleEnemyDeath;
        }
    }
    
    void HandleEnemyDeath()
    {
        enemiesAlive--;
        totalKills++;
        UpdateKillCountUI();
        Debug.Log("Enemy died! Remaining: " + enemiesAlive);
    }

    void UpdateWaveUI()
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + currentWave;
        }
    }

    public int GetTotalKills()
    {
        return totalKills;
    }

    void UpdateKillCountUI()
    {
        if (killCountText != null)
        {
            killCountText.text = "Kills: " + totalKills;
        }
    }

    public void TriggerExtractionWave()
    {
        extractionActive = true;
        
        Debug.Log("EXTRACTION WAVE TRIGGERED!");
        
        // Spawn final wave (more enemies than normal)
        int finalWaveEnemies = enemiesToSpawn * 2; // Double current wave
        
        for (int i = 0; i < finalWaveEnemies; i++)
        {
            SpawnEnemy();
        }
        
        enemiesAlive += finalWaveEnemies;
    }

    void SpawnExtractionPoint()
    {
        // Spawn extraction point near player but not too close
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = player.position + (Vector3)(randomDirection * 8f);
        
        spawnedExtractionPoint = Instantiate(extractionPointPrefab, spawnPosition, Quaternion.identity);
        
        Debug.Log("Extraction point spawned! Find it and hold E to extract!");
    }
}