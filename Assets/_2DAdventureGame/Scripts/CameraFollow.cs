using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector2 minBounds = new Vector2(0f, -2f);
    public Vector2 maxBounds = new Vector2(20f, 3f);

    void LateUpdate()
    {
        if (target == null) return;

        float targetX = Mathf.Clamp(target.position.x, minBounds.x, maxBounds.x);
        float targetY = Mathf.Clamp(target.position.y, minBounds.y, maxBounds.y);

        Vector3 desiredPosition = new Vector3(targetX, targetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}