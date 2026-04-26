using UnityEngine;
public class UnitShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 0.4f;
    public float bulletSpeed = 18f;
    public Transform muzzlePoint;

    private float fireTimer;
    private static CameraFollow camFollow;

    void Start()
    {
        fireTimer = Random.Range(0f, fireRate);
        if(camFollow == null) 
        {
            camFollow =Camera.main.GetComponent<CameraFollow>();
        }
    }

    void Update()
    {
        if(!GameManager.Instance.IsPlaying) return;
        fireTimer -= Time.deltaTime;
        if(fireTimer <= 0f) 
        { 
            Shoot(); fireTimer = fireRate;
        }
    }

    void Shoot()
    {
        Transform spawnAt;
        if(muzzlePoint !=null)
        {
            spawnAt =muzzlePoint;
        }
        else
        {
            spawnAt =transform;
        }

        GameObject b = Instantiate(bulletPrefab, spawnAt.position, Quaternion.identity);
        Rigidbody rb = b.GetComponent<Rigidbody>();
        if (rb != null){
            Vector3 velocity =Vector3.forward * bulletSpeed;
            rb.linearVelocity =velocity;
        }
    }
}