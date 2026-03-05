using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteGlowController : MonoBehaviour, IPointerClickHandler
{
    [Header("发光参数")]
    public Color glowColor = new Color(1f, 1f, 0.2f, 0.2f); // 发光颜色（默认青色）
    [Range(0, 10)] public float glowPower = 3f; // 发光强度
    [Range(0, 0.1f)] public float glowRange = 0.03f; // 发光范围（2D精灵建议0.01~0.05）
    [Range(0, 1)] public float glowThreshold = 0.1f; // 补充Shader需要的阈值参数

    private SpriteRenderer _spriteRenderer;
    private Material _glowMaterial;
    private bool _isSelected = false; // 是否被选中

    void Awake()
    {
        // 获取SpriteRenderer组件，并保存发光材质引用
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _glowMaterial = _spriteRenderer.material; // 实例化材质，避免影响其他对象

        // 关键修复：优先读取材质已有颜色，而非用脚本默认值覆盖
        if (_glowMaterial != null)
        {
            // 从材质读取当前配置，保证Inspector改的颜色/参数生效
            glowColor = _glowMaterial.GetColor("_GlowColor");
            glowPower = _glowMaterial.GetFloat("_GlowPower");
            glowRange = _glowMaterial.GetFloat("_GlowRange");
            glowThreshold = _glowMaterial.GetFloat("_GlowThreshold");

            SetGlowEnabled(false); // 初始关闭发光
        }
        else
        {
            Debug.LogError("SpriteGlowController：未找到发光材质！请检查SpriteRenderer的Material是否赋值", this);
        }
    }

    /// <summary>
    /// 控制发光开关的核心方法
    /// </summary>
    /// <param name="enabled">是否开启发光</param>
    public void SetGlowEnabled(bool enabled)
    {
        if (_glowMaterial == null) return;

        // 修复：完整传递所有Shader参数（包括新增的glowThreshold）
        _glowMaterial.SetColor("_GlowColor", enabled ? glowColor : Color.clear);
        _glowMaterial.SetFloat("_GlowPower", enabled ? glowPower : 0);
        _glowMaterial.SetFloat("_GlowRange", glowRange);
        _glowMaterial.SetFloat("_GlowThreshold", glowThreshold); // 补充传递阈值
    }

    /// <summary>
    /// 监听点击事件：点击切换选中状态
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        _isSelected = !_isSelected;
        SetGlowEnabled(_isSelected);
    }

    /// <summary>
    /// 外部调用方法：手动设置选中状态（比如通过其他按钮控制）
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        SetGlowEnabled(isSelected);
    }

    // 可选：运行时修改脚本参数后，实时同步到材质
    void OnValidate()
    {
        if (_glowMaterial != null)
        {
            _glowMaterial.SetColor("_GlowColor", glowColor);
            _glowMaterial.SetFloat("_GlowPower", glowPower);
            _glowMaterial.SetFloat("_GlowRange", glowRange);
            _glowMaterial.SetFloat("_GlowThreshold", glowThreshold);
        }
    }

    // 销毁时释放材质实例，避免内存泄漏
    void OnDestroy()
    {
        if (_glowMaterial != null)
        {
            Destroy(_glowMaterial);
        }
    }
}