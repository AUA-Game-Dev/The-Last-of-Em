using UnityEngine;
using System.Collections.Generic;

public class SquadManager : MonoBehaviour
{
    public static SquadManager Instance;

    public GameObject unitPrefab;
    public int startingUnits = 5;
    public float unitSpacing = 0.9f;
    public float repositionSpeed = 12f;

    private List<GameObject> units = new List<GameObject>();
    private CameraFollow camFollow;

    public int UnitCount;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        camFollow = Camera.main.GetComponent<CameraFollow>();

        for (int i = 0; i < startingUnits; i++)
        {
            SpawnUnit();
        }

        GameManager.Instance.UpdateHUD();
    }

    void Update()
    {
        RepositionUnits();
    }

    public void AddUnits(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnUnit();
        }

        GameManager.Instance.UpdateHUD();
        camFollow.PulseFOV(5f + count * 0.5f);
    }

    public void MultiplyUnits(int multiplier)
    {
        int toAdd = UnitCount * (multiplier - 1);

        for (int i = 0; i < toAdd; i++)
        {
            SpawnUnit();
        }

        GameManager.Instance.UpdateHUD();
        camFollow.PulseFOV(15f);
    }

    public void RemoveUnits(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (units.Count == 0) break;

            int lastIndex = units.Count - 1;
            Destroy(units[lastIndex]);
            units.RemoveAt(lastIndex);
        }

        UnitCount = units.Count;
        GameManager.Instance.UpdateHUD();

        if (units.Count == 0)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }

    void SpawnUnit()
    {
        GameObject newUnit = Instantiate(unitPrefab, transform.position, Quaternion.identity, transform);
        units.Add(newUnit);
        UnitCount = units.Count;
    }

    void RepositionUnits()
    {
        int count = units.Count;
        float totalWidth = (count - 1) * unitSpacing;

        for (int i = 0; i < count; i++)
        {
            if (units[i] == null) continue;

            float targetX = transform.position.x - totalWidth / 2f + i * unitSpacing;
            float targetY = transform.position.y;
            float targetZ = transform.position.z;

            Vector3 targetPos = new Vector3(targetX, targetY, targetZ);

            units[i].transform.position = Vector3.Lerp(units[i].transform.position, targetPos, repositionSpeed * Time.deltaTime);
        }
    }
}