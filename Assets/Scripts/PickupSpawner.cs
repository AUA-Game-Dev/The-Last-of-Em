using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [System.Serializable]
    public class PickupEntry
    {
        public GameObject prefab;
        public float weight = 1f;
    }

    public PickupEntry[] pickups;
    public float minInterval = 3f;
    public float maxInterval = 6f;
    public float spawnZ = 22f;
    public float spawnXHalfWidth = 3f;

    private float timer;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;

        timer = timer - Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnPickup();
            ResetTimer();
        }
    }

    void SpawnPickup()
    {
        if (pickups == null || pickups.Length == 0) return;

        // Add up all the weights
        float totalWeight = 0f;
        for (int i = 0; i < pickups.Length; i++)
        {
            totalWeight = totalWeight + pickups[i].weight;
        }

        // Pick a random number between 0 and total weight
        float roll = Random.Range(0f, totalWeight);

        // Walk through the list to find which pickup was picked
        float runningTotal = 0f;
        GameObject prefab = pickups[0].prefab;
        for (int i = 0; i < pickups.Length; i++)
        {
            runningTotal = runningTotal + pickups[i].weight;
            if (roll <= runningTotal)
            {
                prefab = pickups[i].prefab;
                break;
            }
        }

        // Spawn it near the player
        float playerX = 0f;
        if (SquadManager.Instance != null)
        {
            playerX = SquadManager.Instance.transform.position.x;
        }

        float x = playerX + Random.Range(-spawnXHalfWidth, spawnXHalfWidth);
        Vector3 spawnPos = new Vector3(x, 0.5f, spawnZ);

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    void ResetTimer()
    {
        timer = Random.Range(minInterval, maxInterval);
    }
}