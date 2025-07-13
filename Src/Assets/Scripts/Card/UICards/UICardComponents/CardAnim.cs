using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 卡牌动画控制器，负责管理卡牌的悬停旋转效果
/// 优化建议：可以将直接获取改为事件通知方式，可能减少性能开销
/// </summary>
public class 卡牌动画 : MonoBehaviour
{
    // 旋转相关参数
    [Header("旋转参数")]
    [SerializeField] private Vector3 rotationCenter = new Vector3(0.0f, -19.8f, 0.0f); // 旋转中心点（世界坐标系）
    [SerializeField] private float angle = 5f;      // 悬停时卡牌的旋转角度偏移量
    [SerializeField] private float lerpSpeed = 5f;   // 动画插值速度

    // 卡牌缓存相关
    [Header("卡牌缓存")]
    [SerializeField] private CardSelection[] ChildrenCards; // 所有子卡牌的CardSelection组件缓存
    [SerializeField] private Vector3[] Positions;     // 卡牌初始位置缓存
    [SerializeField] private Quaternion[] Rotations; // 卡牌初始旋转缓存
    [SerializeField] private Vector3[] TargetPositions; // 卡牌目标位置缓存

    // 内部状态变量
    [Header("内部状态变量")]
    [SerializeField] private int cardOnHover; // 当前悬停的卡牌索引
    [SerializeField] private float RoundTime; // 强制回正时间计数器

    // 卡牌悬停索引属性
    public int CardOnHover
    {
        get { return cardOnHover; }
        set
        {
            if (cardOnHover != value) // 只有当悬停卡牌改变时才执行
            {
                RoundTime = 0; // 重置强制回正计时器
                cardOnHover = value; // 更新悬停卡牌索引
                CountTarget(); // 重新计算所有卡牌的目标位置
            }
        }
    }

    /// <summary>
    /// 每帧更新卡牌状态（使用FixedUpdate保证物理更新频率）
    /// </summary>
    void FixedUpdate()
    {
        // 如果卡牌缓存未初始化，先进行初始化
        if (ChildrenCards == null || ChildrenCards.Length <= 0)
        {
            InitializeCardCache();
            return;
        }

        // 检测当前悬停状态并更新CardOnHover
        CardOnHover = CheckHoverState();

        // 如果所有卡牌都已到达目标位置，则跳过后续处理
        if (CheckBoolCardReady())
        {
            return;
        }

        // 根据是否有卡牌悬停执行不同动画
        if (CardOnHover >= 0)
        {
            RotateAroundCenter(); // 有卡牌悬停时的旋转动画
        }
        else
        {
            ResetRotation(); // 无卡牌悬停时的复位动画
        }
    }

    /// <summary>
    /// 初始化卡牌缓存数据
    /// </summary>
    private void InitializeCardCache()
    {
        // 获取所有子卡牌的CardSelection组件
        ChildrenCards = GetComponentsInChildren<CardSelection>();

        // 初始化各缓存数组
        Positions = new Vector3[ChildrenCards.Length];
        Rotations = new Quaternion[ChildrenCards.Length];
        TargetPositions = new Vector3[ChildrenCards.Length];

        // 缓存初始位置和旋转
        for (int i = 0; i < ChildrenCards.Length; i++)
        {
            Positions[i] = ChildrenCards[i].transform.position;
            Rotations[i] = ChildrenCards[i].transform.rotation;
        }
    }

    /// <summary>
    /// 检测当前悬停的卡牌
    /// </summary>
    /// <returns>悬停卡牌的索引，没有悬停时返回-1</returns>
    private int CheckHoverState()
    {
        // 遍历所有卡牌检测悬停状态
        for (int i = 0; i < ChildrenCards.Length; i++)
        {
            if (ChildrenCards[i].IsOnHover())
            {
                return i; // 返回第一个悬停卡牌的索引
            }
        }
        return -1; // 没有卡牌悬停
    }

    /// <summary>
    /// 使卡牌绕圆心Z轴旋转（悬停状态动画）
    /// </summary>
    private void RotateAroundCenter()
    {
        // 更新强制回正计时器
        RoundTime += Time.deltaTime;

        for (int i = 0; i < ChildrenCards.Length; i++)
        {
            float t = lerpSpeed * Time.deltaTime; // 计算插值系数

            // 根据卡牌位置决定旋转方向（悬停卡牌左侧右旋，右侧左旋）
            float Roundangle = (i < cardOnHover) ? angle : -angle;

            // 设置目标旋转（悬停卡牌不旋转）
            Quaternion targetrotation = (i == cardOnHover) ?
                Quaternion.identity :
                Quaternion.Euler(0, 0, Roundangle);

            // 设置目标缩放（悬停卡牌放大）
            float targetlocalscale = (i == cardOnHover) ? 0.8f : 0.6f;

            // 平滑移动到目标位置（线性插值）                  
            ChildrenCards[i].transform.position = Vector3.Lerp(
                ChildrenCards[i].transform.position,
                TargetPositions[i],
                t);

            // 平滑旋转到目标角度（四元数插值）                    
            ChildrenCards[i].transform.rotation = Quaternion.Lerp(
                ChildrenCards[i].transform.rotation,
                targetrotation,
                t);

            // 平滑缩放
            ChildrenCards[i].transform.localScale = Vector3.Lerp(
                ChildrenCards[i].transform.localScale,
                Vector3.one * targetlocalscale,
                t);

            // 如果超过强制回正时间阈值，直接设置最终状态
            if (RoundTime > 3 * t)
            {
                ChildrenCards[i].transform.position = TargetPositions[i];
                ChildrenCards[i].transform.rotation = targetrotation;
                ChildrenCards[i].transform.localScale = Vector3.one * targetlocalscale;
            }
        }
    }

    /// <summary>
    /// 检查所有卡牌是否都已到达目标位置
    /// </summary>
    /// <returns>所有卡牌都到达目标位置返回true，否则false</returns>
    public bool CheckBoolCardReady()
    {
        for (int i = 0; i < ChildrenCards.Length; i++)
        {
            if (ChildrenCards[i].transform.position != TargetPositions[i])
                return false;
        }
        return true;
    }

    /// <summary>
    /// 计算所有卡牌的目标位置
    /// </summary>
    private void CountTarget()
    {
        // 如果没有卡牌悬停，目标位置就是初始位置
        if (cardOnHover == -1)
        {
            for (int i = 0; i < ChildrenCards.Length; i++)
            {
                TargetPositions[i] = Positions[i];
            }
            return;
        }

        // 计算每张卡牌的目标位置
        for (int i = 0; i < ChildrenCards.Length; i++)
        {
            // 根据卡牌位置决定旋转方向
            float Roundangle = (i < cardOnHover) ? angle : -angle;
            // 计算旋转后的位置
            TargetPositions[i] = RotatePointAroundPivot(Positions[i], rotationCenter, Roundangle);
        }

        // 悬停卡牌稍微上移
        TargetPositions[cardOnHover] = Positions[cardOnHover] + new Vector3(0, 0.4f, 0);
    }

    /// <summary>
    /// 计算点绕旋转中心旋转后的位置
    /// </summary>
    /// <param name="point">原始位置</param>
    /// <param name="pivot">旋转中心</param>
    /// <param name="angle">旋转角度（度）</param>
    /// <returns>旋转后的新位置</returns>
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
    {
        // 创建旋转矩阵
        Matrix4x4 matrix = Matrix4x4.TRS(
            pivot,                  // 旋转中心
            Quaternion.Euler(0, 0, angle), // 旋转角度
            Vector3.one            // 不缩放
        );
        // 应用旋转矩阵
        return matrix.MultiplyPoint3x4(point - pivot);
    }

    /// <summary>
    /// 重置所有卡牌到初始位置和旋转（无悬停状态动画）
    /// </summary>
    private void ResetRotation()
    {
        for (int i = 0; i < ChildrenCards.Length; i++)
        {
            // 使用双倍速度复位
            float t = lerpSpeed * 2f * Time.deltaTime;

            // 对非悬停卡牌应用旋转效果
            if (ChildrenCards[i].transform.position != Positions[i])
            {
                // 平滑移动到初始位置
                ChildrenCards[i].transform.position = Vector3.Lerp(
                    ChildrenCards[i].transform.position,
                    Positions[i],
                    t);

                // 平滑旋转到初始角度
                ChildrenCards[i].transform.rotation = Quaternion.Lerp(
                    ChildrenCards[i].transform.rotation,
                    Rotations[i],
                    t);
            }
            // 平滑缩放到默认大小
            ChildrenCards[i].transform.localScale = Vector3.Lerp(
                ChildrenCards[i].transform.localScale,
                Vector3.one * 0.6f,
                t);
        }
    }
}