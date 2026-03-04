using UnityEngine;

public class CameraController : MonoBehaviour {
    float minVerticalRotation;
    float maxVerticalRotation;
    float verticalRotation;

    void Awake() {
        minVerticalRotation = -5f;
        maxVerticalRotation = 30f;
        verticalRotation = transform.localEulerAngles.x;
    }

    void Update() {
        verticalRotation -= Input.GetAxis("Mouse Y") * Settings.mouseSentityY * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalRotation, maxVerticalRotation);

        Player.instance.transform.Rotate(Input.GetAxis("Mouse X") * Settings.mouseSentityX * Time.deltaTime * Vector3.up);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
