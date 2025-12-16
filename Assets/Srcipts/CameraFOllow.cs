using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Gán player vào đây trong Inspector

    void LateUpdate()
    {
        if (target == null) return;

        // Lấy vị trí của player
        Vector3 newPos = target.position;

        // Giữ nguyên trục Z của camera (để không bị lệch trong game 2D)
        newPos.z = transform.position.z;

        // Gán vị trí mới cho camera
        transform.position = newPos;
    }
}
