using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// 特效组
/// </summary>
public class VFXComponent : VFXBase
{
    [Tooltip("组合ID")]
    public string CompositionId;

    [Tooltip("包含的组件")]
    public List<VFXBase> ChildComponents = new List<VFXBase>();
    public List<VFXConfig> CompsConfigs = new List<VFXConfig>();

    [Tooltip("组件延迟时间")]
    public float[] ComponentDelays;
    public float TotalDelay;

    [Tooltip("组件持续时间乘数")]
    public float[] ComponentDurationMultipliers;

    /// <summary>
    /// 初始化组合
    /// </summary>
    public void InitializeComposition()
    {
        // 收集所有子组件
        ChildComponents.Clear();
        CompsConfigs.Clear();
        foreach (Transform child in transform)
        {
            VFXBase component = child.GetComponent<VFXBase>();
            if (component != null)
            {
                ChildComponents.Add(component);
            }
        }

        // 初始化延迟和持续时间数组
        ComponentDelays = new float[ChildComponents.Count];
        ComponentDurationMultipliers = new float[ChildComponents.Count];

        for (int i = 0; i < ChildComponents.Count; i++)
        {
            ComponentDelays[i] = 0f;
            ComponentDurationMultipliers[i] = 1f;
        }
    }

    public void SetConfigs(List<VFXConfig> configs)
    {
        CompsConfigs = configs;
    }




    /// <summary>
    /// 播放轨迹特效
    /// </summary>
    public override void PlayTrailEffect(Vector3 start, Vector3 end, float duration)
    {
        base.PlayTrailEffect(start, end, duration);

        // 为每个子组件计算参数并播放
        for (int i = 0; i < ChildComponents.Count; i++)
        {
            VFXBase component = ChildComponents[i];

            // 计算组件特定的持续时间
            float componentDuration = duration * ComponentDurationMultipliers[i];

            // 使用协程处理延迟播放
            if (ComponentDelays[i] > 0)
            {
                StartCoroutine(DelayedPlayComponent(component, start, end, componentDuration, ComponentDelays[i]));
            }
            else
            {
                component.PlayTrailEffect(start, end, componentDuration);
            }
        }
    }

    /// <summary>
    /// 播放定点特效
    /// </summary>
    public override void PlayPointEffect(Vector3 position, float duration)
    {
        base.PlayPointEffect(position, duration);

        // 为每个子组件计算参数并播放
        for (int i = 0; i < ChildComponents.Count; i++)
        {
            VFXBase component = ChildComponents[i];

            // 计算组件特定的持续时间
            float componentDuration = duration * ComponentDurationMultipliers[i];

            // 使用协程处理延迟播放
            if (ComponentDelays[i] > 0)
            {
                StartCoroutine(DelayedPlayComponent(component, position, componentDuration, ComponentDelays[i]));
            }
            else
            {
                component.PlayPointEffect(position, componentDuration);
            }
        }
    }

    /// <summary>
    /// 设置组件延迟时间
    /// </summary>
    public void SetComponentDelay(int index, float delay)
    {
        if (index >= 0 && index < ComponentDelays.Length)
        {
            ComponentDelays[index] = delay;
        }
    }

    /// <summary>
    /// 设置组件持续时间乘数
    /// </summary>
    public void SetComponentDurationMultiplier(int index, float multiplier)
    {
        if (index >= 0 && index < ComponentDurationMultipliers.Length)
        {
            ComponentDurationMultipliers[index] = multiplier;
        }
    }

    /// <summary>
    /// 设置所有组件的颜色
    /// </summary>
    public void SetAllParticleColors(Color color)
    {
        foreach (VFXBase component in ChildComponents)
        {
            component.SetColor(color);
        }
    }

    /// <summary>
    /// 设置所有组件的粒子大小
    /// </summary>
    public void SetAllParticleSizes(float size)
    {
        foreach (VFXBase component in ChildComponents)
        {
            component.SetSize(size);
        }
    }

    /// <summary>
    /// 延迟播放组件
    /// </summary>
    private IEnumerator DelayedPlayComponent(VFXBase component, Vector3 start, Vector3 end, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        component.PlayTrailEffect(start, end, duration);
    }

    /// <summary>
    /// 延迟播放组件
    /// </summary>
    private IEnumerator DelayedPlayComponent(VFXBase component, Vector3 position, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        component.PlayPointEffect(position, duration);
    }

    /// <summary>
    /// 更新特效状态
    /// </summary>
    public void Update()
    {

        // 检查所有子组件是否完成
        if (!IsFinished)
        {
            bool allFinished = true;

            foreach (VFXBase component in ChildComponents)
            {
                if (!component.IsFinished)
                {
                    allFinished = false;
                    break;
                }
            }

            if (allFinished)
            {
                IsFinished = true;
            }
        }
    }
}