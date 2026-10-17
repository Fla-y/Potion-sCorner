using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField, Min(0f)] private float smoothTime = 0.15f;

    [Tooltip("Optional: assign the Renderer of the full background to keep the camera inside its bounds.")]
    [SerializeField] private Renderer worldBounds;

    private Camera cameraComponent;
    private Vector3 followVelocity;
    private float cameraZ;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();
        cameraZ = transform.position.z;

        if (!cameraComponent.orthographic)
        {
            Debug.LogError("CameraFollow2D requires an orthographic camera.", this);
            enabled = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (target == null)
        {
            Debug.LogError("CameraFollow2D: assign the player Transform to Target.", this);
            enabled = false;
            return;
        }

        SnapToTarget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desired = GetTargetPosition();
        Vector3 position = smoothTime > 0f
            ? Vector3.SmoothDamp(transform.position, desired, ref followVelocity,
                smoothTime, Mathf.Infinity, Time.deltaTime)
            : desired;

        transform.position = ClampToWorld(position);
    }

    public void SnapToTarget()
    {
        if (target == null || cameraComponent == null)
            return;

        followVelocity = Vector3.zero;
        transform.position = GetTargetPosition();
    }

    private Vector3 GetTargetPosition()
    {
        return ClampToWorld(new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            cameraZ));
    }

    private Vector3 ClampToWorld(Vector3 position)
    {
        position.z = cameraZ;
        if (worldBounds == null)
            return position;

        Bounds bounds = worldBounds.bounds;
        float halfHeight = cameraComponent.orthographicSize;
        float halfWidth = halfHeight * cameraComponent.aspect;

        position.x = bounds.size.x <= halfWidth * 2f
            ? bounds.center.x
            : Mathf.Clamp(position.x, bounds.min.x + halfWidth, bounds.max.x - halfWidth);
        position.y = bounds.size.y <= halfHeight * 2f
            ? bounds.center.y
            : Mathf.Clamp(position.y, bounds.min.y + halfHeight, bounds.max.y - halfHeight);

        return position;
    }
}
