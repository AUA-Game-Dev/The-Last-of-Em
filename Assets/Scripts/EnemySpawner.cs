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

        waveTimer += Time.deltaTime;
        if (waveTimer >= waveDuration)
        {
            waveTimer = 0f;
            currentWave++;
            currentInterval = Mathf.Max(spawnInterval - (currentWave - 1) * intervalDecrement, minInterval);
            GameManager.Instance.OnWaveChanged(currentWave);
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            int count = 1 + currentWave / 3;
            for (int i = 0; i < count; i++)
                SpawnEnemy();
            spawnTimer = currentInterval;
        }

        if (currentWave >= 4)
            manager.TriggerGameOver();
    }

    void SpawnEnemy()
    {
        if (enemyTypes == null || enemyTypes.Length == 0) return;

        GameObject prefab = PickRandomEnemy();

        float x = Random.Range(-spawnXHalfWidth, spawnXHalfWidth);

        // Spawn at y=0, rotated 180 on Y so the model faces toward the player
        Vector3 spawnPos = new Vector3(x, -1f, spawnZ);
        Quaternion spawnRot = Quaternion.Euler(0f, 180f, 0f);

        Instantiate(prefab, spawnPos, spawnRot);
    }

    GameObject PickRandomEnemy()
    {
        float totalWeight = 0f;
        for (int i = 0; i < enemyTypes.Length; i++)
            totalWeight += enemyTypes[i].weight;

        float roll = Random.Range(0f, totalWeight);
        float runningTotal = 0f;

        for (int i = 0; i < enemyTypes.Length; i++)
        {
            runningTotal += enemyTypes[i].weight;
            if (roll <= runningTotal)
                return enemyTypes[i].prefab;
        }

        return enemyTypes[0].prefab;
    }
}