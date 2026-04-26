using UnityEngine;
using TMPro;

public class Pickup : MonoBehaviour
{
    //So far 2 types (But we have implemented only the adding one so far, will add the multiplier later)
    public enum PickupType { 
        AddUnits, 
        MultiplyUnits
    }

    public PickupType pickupType = PickupType.AddUnits;
    public int value = 1;

    public float scrollSpeed = 3f;

    public TextMeshPro label;

    void Start()
    {
        //This part is currently under construction. I'm trying to see how I can get the labels to always face the camera
        /**
        if (label != null)
        {
            if (pickupType == PickupType.MultiplyUnits)
            {
                label.text = "x" + value;
            }
            else
            {
                label.text = "+" + value;
            }
        }
        **/
    }

    void Update()
    {
        transform.Translate(Vector3.back * scrollSpeed * Time.deltaTime, Space.World);
        if (transform.position.z < -6f) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            ApplyEffect();
            Destroy(gameObject);
        }
    }

    void ApplyEffect()
    {
        if (SquadManager.Instance == null) return;
        if (pickupType == PickupType.AddUnits)
            SquadManager.Instance.AddUnits(value);
        else
            SquadManager.Instance.MultiplyUnits(value);
    }
}
