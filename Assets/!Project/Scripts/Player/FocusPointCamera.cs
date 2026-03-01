using UnityEngine;

public class FocusPointCamera : MonoBehaviour
{
    [Header("Focus Point Settings")]
    [SerializeField] private Transform focusPoint;

    [Header("Camera Orbit Settings")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 20f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 8f;
    [SerializeField] private float zoomSmoothness = 8f;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask collisionMask = -1; // Все слои по умолчанию

    private float currentYaw = 0f;
    private float currentPitch = 30f;
    private float currentDistance;
    private float targetDistance;

    void Awake()
    {
        currentDistance = distance;
        targetDistance = distance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Вращение камеры вокруг персонажа (стандартный Unity Input)
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        currentYaw += mouseX * rotationSpeed * Time.deltaTime;
        currentPitch -= mouseY * rotationSpeed * Time.deltaTime;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        // Зум через колесо мыши
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        targetDistance -= scrollInput * zoomSpeed * 10f;
        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * zoomSmoothness);
    }

    void LateUpdate()
    {
        if (focusPoint == null) return;

        // Орбитальное вращение: камера летает вокруг персонажа
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 direction = rotation * Vector3.back;
        Vector3 targetPosition = focusPoint.position + direction * currentDistance;

        // Защита от стен (камера не проходит сквозь препятствия)
        if (Physics.Linecast(focusPoint.position, targetPosition, out RaycastHit hit, collisionMask))
        {
            targetPosition = hit.point + hit.normal * 0.2f;
        }

        transform.position = targetPosition;
        transform.LookAt(focusPoint.position);
    }

    public void SetFocusPoint(Transform newFocus) => focusPoint = newFocus;
}