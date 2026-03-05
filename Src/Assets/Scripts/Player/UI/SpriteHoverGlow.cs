using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 鼠标悬浮发光、离开关闭的控制脚本（适配双Sprite层方案）
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SpriteHoverGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("赋值发光子对象")]
    public GameObject glowLayer; // 拖入你的PlayerGlow子对象

    void Start()
    {
        // 初始关闭发光层
        if (glowLayer != null)
        {
            glowLayer.SetActive(false);
        }
    }

    // 鼠标悬浮时激活发光层
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (glowLayer != null)
        {
            glowLayer.SetActive(true);
        }
    }

    // 鼠标离开时关闭发光层
    public void OnPointerExit(PointerEventData eventData)
    {
        if (glowLayer != null && !_isSelected) // 若有选中逻辑，需保留_isSelected
        {
            glowLayer.SetActive(false);
        }
    }

    // 可选：点击选中后持续发光
    private bool _isSelected = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        _isSelected = !_isSelected;
        glowLayer.SetActive(_isSelected);
    }
}