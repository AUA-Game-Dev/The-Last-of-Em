using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = new Vector3(0f, 6f, -8f);
    public float positionSmoothSpeed = 8f;
    public float xLagMultiplier = 0.6f;

    public float baseFOV = 60f;
    public float fovSmoothSpeed = 5f;
    private float targetFOV;
    private Camera cam;

    public float maxTiltAngle = 3f;
    public float tiltSmoothSpeed = 6f;

    public float bobAmount = 0.04f;
    public float bobSpeed = 30f;
    private float bobTimer = 0f;
    private bool isBobbing = false;

    private float prevX;
    private float currentVelocityX;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetFOV = baseFOV;
        cam.fieldOfView = baseFOV;
        if (target != null)
        {
            prevX = target.position.x;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        float desiredX = target.position.x + offset.x;
        float desiredY = target.position.y + offset.y;
        float desiredZ = target.position.z + offset.z;

        float smoothX = Mathf.Lerp(transform.position.x, desiredX, positionSmoothSpeed * xLagMultiplier * Time.deltaTime);
        float smoothY = Mathf.Lerp(transform.position.y, desiredY, positionSmoothSpeed * Time.deltaTime);
        float smoothZ = Mathf.Lerp(transform.position.z, desiredZ, positionSmoothSpeed * Time.deltaTime);

        transform.position = new Vector3(smoothX, smoothY, smoothZ);

        transform.LookAt(target.position + Vector3.forward * 3f);

        currentVelocityX = (target.position.x - prevX) / Time.deltaTime;
        prevX = target.position.x;

        float tiltTarget = -currentVelocityX * 0.8f;

        if (tiltTarget > maxTiltAngle) tiltTarget = maxTiltAngle;
        if (tiltTarget < -maxTiltAngle) tiltTarget = -maxTiltAngle;

        float currentTiltRaw = transform.eulerAngles.z;
        float currentTilt = currentTiltRaw;
        if (currentTilt > 180f)
        {
            currentTilt = currentTilt - 360f;
        }

        float newTilt = Mathf.Lerp(currentTilt, tiltTarget, tiltSmoothSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, newTilt);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovSmoothSpeed * Time.deltaTime);

        //Bob the camera up and down when shooting (Experimaental, may remove later)
        if (isBobbing == true)
        {
            bobTimer = bobTimer + Time.deltaTime * bobSpeed;
            float bob = Mathf.Sin(bobTimer) * bobAmount;
            transform.position = transform.position + Vector3.up * bob;

            if (bobTimer > Mathf.PI)
            {
                isBobbing = false;
                bobTimer = 0f;
            }
        }
    }

    public void PulseFOV(float extraFOV = 8f)
    {
        targetFOV = baseFOV + extraFOV;
        Invoke(nameof(ResetFOV), 0.08f);
    }

    void ResetFOV()
    {
        targetFOV = baseFOV;
    }

    public void TriggerBob()
    {
        if (isBobbing == false)
        {
            isBobbing = true;
            bobTimer = 0f;
        }
    }
}