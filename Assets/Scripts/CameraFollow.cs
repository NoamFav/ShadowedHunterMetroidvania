using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player to follow
    public float smoothSpeed = 0.125f; // Adjust for smoother or snappier movement
    public Vector3 offset = new Vector3(0, 2, -10); // Default camera offset

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired position
            Vector3 desiredPosition = target.position + offset;

            // Smoothly move the camera towards the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Set the camera's position to the smoothed position
            transform.position = smoothedPosition;
        }
    }
}
