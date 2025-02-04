using UnityEngine;

public class ParallaxEffect : MonoBehaviour {
    public float parallaxSpeed = 0.02f;
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    void Start() {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void Update() {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxSpeed, 0, 0);
        lastCameraPosition = cameraTransform.position;
    }
}
