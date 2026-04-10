using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public Camera camera; // 引用主摄像机
    public SpriteRenderer spriteRenderer; // 引用SpriteRenderer
    public float scrollSpeed = 10f; // 滚轮控制速度
    public float minZoom = 5f; // 最小摄像机视野
    public float maxZoom = 20f; // 最大摄像机视野
    [SerializeField] private Transform relicPanel; // 跟随摄像机移动的遗物面板

    private float originalSize;
    private Vector3 relicPanelOffset;

    void Start()
    {
        // 获取摄像机原始的视野大小（根据需求，可以使用orthographicSize来控制2D摄像机）
        originalSize = camera.orthographicSize;

        if (relicPanel == null && RelicDisplay.Instance != null)
        {
            relicPanel = RelicDisplay.Instance.transform;
        }

        if (relicPanel != null)
        {
            relicPanelOffset = relicPanel.position - camera.transform.position;
        }
    }

    void Update()
    {
        // 控制摄像机的缩放（使用滚轮）
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        camera.orthographicSize -= scroll * scrollSpeed;

        // 限制摄像机的视野
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoom, maxZoom);

        // 计算Sprite的宽高，确定摄像机的移动范围（仅用 Y 范围）
        Vector2 spriteSize = spriteRenderer.bounds.size;
        float maxCameraY = spriteSize.y - camera.orthographicSize;

        // 控制摄像机位置的边界，确保摄像机不会移动到超出Sprite范围的地方（仅限制 Y 轴）
        Vector3 cameraPosition = camera.transform.position;
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, 0, maxCameraY);
        // cameraPosition.x 保持不变，不沿 X 轴移动

        // 更新摄像机位置
        camera.transform.position = cameraPosition;

        // 让遗物面板和摄像机一起移动（保持初始相对偏移）
        if (relicPanel != null)
        {
            relicPanel.position = camera.transform.position + relicPanelOffset;
        }

        // 可以按键或鼠标拖动来手动移动摄像机（可选）- 仅沿 Y 轴移动
        if (Input.GetMouseButton(1))  // 右键按下时
        {
            float moveY = Input.GetAxis("Mouse Y");
            camera.transform.Translate(new Vector3(0, -moveY, 0));
        }
    }
}