using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public Camera camera; // 引用主摄像机
    public SpriteRenderer spriteRenderer; // 引用SpriteRenderer
    public float dragSpeed = 1f; // 鼠标拖动摄像机速度
    [SerializeField] private Transform relicPanel; // 跟随摄像机移动的遗物面板
    [SerializeField] private Transform eventBackground; // 跟随摄像机移动的事件背景

    private Vector3 relicPanelOffset;
    private Vector3 eventBackgroundOffset;
    private bool relicPanelOffsetInitialized;

    void Start()
    {
        ResolveRelicPanel();
        if (eventBackground == null && EventManager.Instance != null)
        {
            eventBackground = EventManager.Instance.EventBackgroundTransform;
        }

        TryCaptureRelicPanelOffset();
        if (eventBackground != null)
        {
            eventBackgroundOffset = eventBackground.position - camera.transform.position;
        }
    }

    void ResolveRelicPanel()
    {
        if (relicPanel != null) return;
        if (RelicDisplay.Instance != null && RelicDisplay.Instance.RelicContainer != null)
        {
            relicPanel = RelicDisplay.Instance.RelicContainer;
        }
    }

    void TryCaptureRelicPanelOffset()
    {
        if (relicPanel == null || camera == null) return;
        relicPanelOffset = relicPanel.position - camera.transform.position;
        relicPanelOffsetInitialized = true;
    }

    void Update()
    {
        if (!relicPanelOffsetInitialized)
        {
            ResolveRelicPanel();
            TryCaptureRelicPanelOffset();
        }

        // 右键或中键按下时，沿 Y 轴拖动摄像机（滚轮不再缩放）
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            float moveY = Input.GetAxis("Mouse Y");
            camera.transform.Translate(new Vector3(0, -moveY * dragSpeed, 0));
        }

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
        if (relicPanel != null && relicPanelOffsetInitialized)
        {
            relicPanel.position = camera.transform.position + relicPanelOffset;
        }
        if (eventBackground != null)
        {
            eventBackground.position = camera.transform.position + eventBackgroundOffset;
        }
    }
}