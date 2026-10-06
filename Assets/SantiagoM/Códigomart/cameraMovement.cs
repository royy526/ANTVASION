using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.15f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = cam.orthographicSize * cam.aspect;

        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX + camHalfWidth, maxX - camHalfWidth);

        smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY + camHalfHeight, maxY - camHalfHeight);

        transform.position = smoothedPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
        Gizmos.DrawWireCube(center, size);
    }
}