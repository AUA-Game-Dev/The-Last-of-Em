using UnityEngine;
using System.Collections.Generic;

public class SquadManager : MonoBehaviour
{
    public static SquadManager Instance;

    public GameObject unitPrefab;
    public int startingUnits = 5;
    public float unitSpacing = 0.9f;
    public float repositionSpeed = 12f;

    public float wallLeft = -4f; 
    public float wallRight = 4f;  

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

    Vector3 GetHexOffset(int slotIndex)
    {
        if (slotIndex == 0)
        {
            return Vector3.zero;
        }

        int ring = 1;
        int slotsUsed = 1; 
        while (slotsUsed + 6 * ring <= slotIndex)
        {
            slotsUsed = slotsUsed + 6 * ring;
            ring = ring + 1;
        }

        int posInRing = slotIndex - slotsUsed;
        int slotsInRing = 6 * ring;

        float angle = (360f / slotsInRing) * posInRing;
        float rad = angle * Mathf.Deg2Rad;

        float x = Mathf.Cos(rad) * ring * unitSpacing;
        float z = Mathf.Sin(rad) * ring * unitSpacing;

        return new Vector3(x, 0f, z);
    }

    void RepositionUnits()
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] == null) continue;

            Vector3 hexOffset = GetHexOffset(i);
            float idealX = transform.position.x + hexOffset.x;
            float idealZ = transform.position.z + hexOffset.z;

            float clampedX = Mathf.Clamp(idealX, wallLeft, wallRight);

            units[i].transform.position = Vector3.Lerp(
                units[i].transform.position,
                new Vector3(clampedX, transform.position.y, idealZ),
                repositionSpeed * Time.deltaTime
            );
        }
    }
}