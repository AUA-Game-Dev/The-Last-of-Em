using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float laneHalfWidth = 4f;

    private float targetX;
    private bool isDragging = false;
    private float lastTouchX;
    private Animator[] unitAnimators;
    private float smoothedVelocity = 0f;

    void Start()
    {
        targetX = transform.position.x;
    }

    void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;
        HandleMobileInput();
        MoveToTarget();
        UpdateUnitAnimations();
    }

    void HandleMobileInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            bool touchingUI = UnityEngine.EventSystems.EventSystem.current != null &&
                              UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId);
            if (touchingUI) return;

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastTouchX = touch.position.x;
            }
            if (touch.phase == TouchPhase.Moved && isDragging)
            {
                float deltaX = touch.position.x - lastTouchX;
                float worldDelta = deltaX * (laneHalfWidth * 2f / Screen.width);
                targetX = Mathf.Clamp(targetX + worldDelta, -laneHalfWidth, laneHalfWidth);
                lastTouchX = touch.position.x;
            }
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
                targetX = transform.position.x;
            }
        }
        else
        {
            bool clickingUI = UnityEngine.EventSystems.EventSystem.current != null &&
                              UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastTouchX = Input.mousePosition.x;
            }
            if (Input.GetMouseButton(0) && isDragging && !clickingUI)
            {
                float deltaX = Input.mousePosition.x - lastTouchX;
                float worldDelta = deltaX * (laneHalfWidth * 2f / Screen.width);
                targetX = Mathf.Clamp(targetX + worldDelta, -laneHalfWidth, laneHalfWidth);
                lastTouchX = Input.mousePosition.x;
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                targetX = transform.position.x;
            }
        }

        float h = Input.GetAxis("Horizontal");
        if (Mathf.Abs(h) > 0.01f)
            targetX = Mathf.Clamp(targetX + h * moveSpeed * Time.deltaTime, -laneHalfWidth, laneHalfWidth);
    }

    void MoveToTarget()
    {
        float smoothX = Mathf.Lerp(transform.position.x, targetX, 25f * Time.deltaTime);
        transform.position = new Vector3(smoothX, transform.position.y, transform.position.z);
    }

    void UpdateUnitAnimations()
    {
        float rawVelocity = targetX - transform.position.x;
        smoothedVelocity = Mathf.Lerp(smoothedVelocity, rawVelocity, 15f * Time.deltaTime);

        // Positive = walk right, negative = walk right mirrored (looks like walk left)
        // Zero = animation paused
        if (SquadManager.Instance == null) return;
        unitAnimators = SquadManager.Instance.GetComponentsInChildren<Animator>();

        foreach (Animator anim in unitAnimators)
        {
            if (anim == null) continue;
            anim.SetFloat("Speed", smoothedVelocity);
        }
    }
}