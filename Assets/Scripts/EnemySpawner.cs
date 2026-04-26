using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyEntry
    {
        public GameObject prefab;
        public float weight = 1f;
    }

    public EnemyEntry[] enemyTypes;

    public float spawnInterval = 2f;
    public float intervalDecrement = 0.1f;
    public float minInterval = 0.5f;
    public float waveDuration = 18f;

    public float spawnZ = 22f;
    public float spawnXHalfWidth = 3.5f;

    private float spawnTimer;
    private float waveTimer;
    public int currentWave = 1;
    private float currentInterval;

    public GameManager manager;

    void Start()
    {
        currentInterval = spawnInterval;
        spawnTimer = currentInterval;
    }

    void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;

        waveTimer = waveTimer + Time.deltaTime;
        if (waveTimer >= waveDuration)
        {
            waveTimer = 0f;
            currentWave = currentWave + 1;

            currentInterval = spawnInterval - (currentWave - 1) * intervalDecrement;
            if (currentInterval < minInterval)
            {
                currentInterval = minInterval;
            }

            GameManager.Instance.OnWaveChanged(currentWave);
        }

        spawnTimer = spawnTimer - Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            int count = 1 + currentWave / 3;
            for (int i = 0; i < count; i++)
            {
                SpawnEnemy();
            }
            spawnTimer = currentInterval;
        }

        //Temporary goal: survive 3 waves
        if (currentWave >= 4)
        {
            manager.RestartGame();
        }
    }

    void SpawnEnemy()
    {
        if (enemyTypes == null || enemyTypes.Length == 0) return;

        GameObject prefab = PickRandomEnemy();

        float playerX = 0f;
        if (SquadManager.Instance != null)
        {
            playerX = SquadManager.Instance.transform.position.x;
        }

        float x = Random.Range(-spawnXHalfWidth, spawnXHalfWidth);
        Vector3 spawnPos = new Vector3(playerX + x, 0f, spawnZ);

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    GameObject PickRandomEnemy()
    {
        float totalWeight = 0f;
        for (int i = 0; i < enemyTypes.Length; i++)
        {
            totalWeight = totalWeight + enemyTypes[i].weight;
        }

        float roll = Random.Range(0f, totalWeight);

        float runningTotal = 0f;
        for (int i = 0; i < enemyTypes.Length; i++)
        {
            runningTotal = runningTotal + enemyTypes[i].weight;
            if (roll <= runningTotal)
            {
                return enemyTypes[i].prefab;
            }
        }

        //Fallback because I'm getting an error
        return enemyTypes[0].prefab;
    }
}