using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 10f;
    public float edgeThreshold = 50f;
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 30f;

    private float defaultOrthographicSize; // 默认正交相机大小
    private float defaultFieldOfView; // 默认透视相机视野
    private Vector3 defaultPosition; // 默认位置

    private float minX = -100f;
    private float maxX = 100f;
    private float minY = -100f;
    private float maxY = 100f;

    private Camera cam;
    private Transform target; // 玩家目标

    private bool isDragging = false;
    private Vector3 dragOrigin;

    void Start()
    {
        cam = Camera.main;

        // 记录默认的相机设置
        defaultOrthographicSize = cam.orthographicSize;
        defaultFieldOfView = cam.fieldOfView;
        defaultPosition = transform.position; // 记录默认位置
    }

    void Update()
    {
        if (GameManager.Instance.IsInPlayMode())
        {
            FollowPlayer(); // 在游戏模式下跟随玩家
        }
        else
        {
            HandleZoom();
            HandleDrag();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget; // 设置新的跟随目标

        // 恢复相机缩放状态为默认值
        if (cam.orthographic)
        {
            cam.orthographicSize = defaultOrthographicSize;
        }
        else
        {
            cam.fieldOfView = defaultFieldOfView;
        }
    }

    public void ResetCameraToDefault()
    {
        // 恢复相机到默认位置和缩放值
        transform.position = defaultPosition;
        if (cam.orthographic)
        {
            cam.orthographicSize = defaultOrthographicSize;
        }
        else
        {
            cam.fieldOfView = defaultFieldOfView;
        }
    }

    private void FollowPlayer()
    {
        if (target != null)
        {
            Vector3 newPosition = target.position;
            newPosition.z = transform.position.z; // 保持相机的Z轴位置不变

            // 限制相机边界
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            transform.position = newPosition;
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (cam.orthographic) // 正交模式下
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
        else // 透视模式下
        {
            cam.fieldOfView -= scroll * zoomSpeed;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
        }
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(2)) // 点击滚轮
        {
            isDragging = true;
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(2) && isDragging) // 拖拽画面时
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPosition = transform.position + difference;

            // 限制相机边界
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            transform.position = newPosition;
        }

        if (Input.GetMouseButtonUp(2)) // 拖拽结束
        {
            isDragging = false;
        }
    }
}
