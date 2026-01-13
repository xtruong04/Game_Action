using UnityEngine;

public class RainFollowCamera : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(
            cam.position.x,
            cam.position.y + 5f,
            transform.position.z
        );
    }
}
