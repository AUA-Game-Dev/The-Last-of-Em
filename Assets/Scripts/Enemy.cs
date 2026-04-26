using UnityEngine;
public class Enemy : MonoBehaviour
{
    public int maxHp = 1;
    public float moveSpeed = 3f;
    public int damage = 1;

    private int currentHp;

    void Start() {
        currentHp = maxHp;
    }

    void Update()
    {
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);
        //Despawn them when they get behind the squad
        if(transform.position.z < -6f)
            Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        currentHp -=amount;
        if (currentHp <=0)
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(10);
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit")) {
            if(SquadManager.Instance != null)
            {
                SquadManager.Instance.RemoveUnits(damage);
            }
            Destroy(gameObject);
        }
    }
}
