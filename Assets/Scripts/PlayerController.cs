using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float laneHalfWidth = 4f;

    private Camera mainCam;
    private Plane groundPlane;
    private float targetX;

    private bool isDragging = false;
    private float lastTouchX;

    void Start()
    {
        mainCam = Camera.main;
        groundPlane = new Plane(Vector3.up, Vector3.zero);
        targetX = transform.position.x;
    }

    void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;

        HandleMobileInput();
        MoveToTarget();
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

            if (touch.phase == TouchPhase.Moved)
            {
                if (isDragging == true)
                {
                    float deltaX = touch.position.x - lastTouchX;
                    float worldDelta = deltaX * (laneHalfWidth * 2f / Screen.width);

                    targetX = targetX + worldDelta;

                    if (targetX > laneHalfWidth) targetX = laneHalfWidth;
                    if (targetX < -laneHalfWidth) targetX = -laneHalfWidth;

                    lastTouchX = touch.position.x;
                }
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            bool clickingUI = UnityEngine.EventSystems.EventSystem.current != null &&
                              UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            if (clickingUI) return;

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            float enter = 0f;

            if (groundPlane.Raycast(ray, out enter))
            {
                float wx = ray.GetPoint(enter).x;

                targetX = wx;

                if (targetX > laneHalfWidth) targetX = laneHalfWidth;
                if (targetX < -laneHalfWidth) targetX = -laneHalfWidth;
            }
        }

        float h = Input.GetAxis("Horizontal");
        if (h > 0.01f || h < -0.01f)
        {
            targetX = targetX + h * moveSpeed * Time.deltaTime;

            if (targetX > laneHalfWidth) targetX = laneHalfWidth;
            if (targetX < -laneHalfWidth) targetX = -laneHalfWidth;
        }
    }

    void MoveToTarget()
    {
        float smoothX = Mathf.Lerp(transform.position.x, targetX, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(smoothX, transform.position.y, transform.position.z);
    }
}