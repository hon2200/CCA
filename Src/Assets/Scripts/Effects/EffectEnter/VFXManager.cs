using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

/// <summary>
/// 特效管理器
/// <para>如果你了解了这个代码的话，可以把PlayTrailEffect和PlayPointEffect统一改为PlayEffect，这样调用就方便很多</para>
/// <para>目前一共新增四个类以及一个配表json</para>
/// <para>VFXConfigDictionary用于读取配表（可以合并不用）</para>
/// <para>VFXManager（管理预制体以及调配和接口和生命周期）</para>
/// <para>VFXComponent（组合，用于统合一个复杂的合并特效，例如可以同时播放大量的特效并统一管理，鉴于我们游戏玩法的特殊性，甚至可以将一轮的特效都放入一个组合统一管理）</para>
/// <para>VFXBase（最基本的特效方法，用于管理与粒子系统之间的调配）</para>
/// </summary>

public class VFXManager : MonoSingleton<VFXManager>
{
    [Tooltip("组件预制体字典")]
    public SerializedDictionary<string, VFXBase> TrailDictionary = new SerializedDictionary<string, VFXBase>();
    public SerializedDictionary<string, VFXBase> SpotDictionary = new SerializedDictionary<string, VFXBase>();

    [Tooltip("组合预制体字典")]
    public SerializedDictionary<string, VFXComponent> ComposDictionary = new SerializedDictionary<string, VFXComponent>();

    [Tooltip("单特效配置字典")]
    public Dictionary<string, VFXConfig> configs = new Dictionary<string, VFXConfig>();
    public Dictionary<string, List<VFXConfig>> composconfigs = new Dictionary<string, List<VFXConfig>>();

    [Tooltip("活跃特效列表")]
    public List<VFXBase> activeEffects = new List<VFXBase>();

    [Tooltip("委托调用")]
    public UnityEvent onPlayEffect = new UnityEvent();

    #region 公共API
    /// <summary>
    /// 播放所有特效
    /// </summary>
    public void PlayAll()
    {
        onPlayEffect?.Invoke();
        onPlayEffect.RemoveAllListeners();
    }
    /// <summary>
    /// 播放轨迹特效
    /// <para>playnow : 是否立刻播放</para>
    /// <para>id : 特效ID</para>
    /// <para>id : 特效ID</para>
    /// <para>start : 起点</para>
    /// <para>end : 终点</para>
    /// <para>duration : 持续时间 , 默认为0.7</para>
    /// <para>Delay : 延迟时间</para>
    /// <para>offset : 相对目标的偏移量</para>
    /// </summary>
    public VFXBase PlayTrailEffect(bool playnow, string id, GameObject origin ,GameObject target, float duration=0.7f, float Delay = 0, Vector3 offset = new Vector3())
    {
        VFXBase effect = null;
        Vector3 targetposition = target.transform.position + offset;
        Vector3 originposition = origin.transform.position;
        if (ComposDictionary.TryGetValue(id,out VFXComponent Trailcompos))
        {
            VFXComponent composition = Instantiate(Trailcompos, originposition, Quaternion.LookRotation(targetposition - originposition));
            composition.TotalDelay = Delay;
            activeEffects.Add(composition);
            effect = composition;
        }
        else if (composconfigs.TryGetValue(id, out List<VFXConfig> composconfig))
        {
            VFXComponent component = new VFXComponent();
            component = Instantiate(component, originposition, Quaternion.LookRotation(targetposition - originposition));
            component.SetConfigs(composconfig);
            component.SetDelay(Delay);
            activeEffects.Add(component);
            effect = component;
        }
        else if (TrailDictionary.TryGetValue(id, out VFXBase TrailBase))
        {
            VFXBase component = Instantiate(TrailBase, originposition, Quaternion.LookRotation(targetposition - originposition));
            component.SetDelay(Delay);
            activeEffects.Add(component);
            effect = component;
        }
        else if (configs.TryGetValue(id,out VFXConfig vFXConfig))
        {
            VFXBase component = Instantiate(TrailDictionary[vFXConfig.id], originposition, Quaternion.LookRotation(targetposition - originposition));
            component.SetConfig(vFXConfig);
            component.SetDelay(Delay);
            activeEffects.Add(component);
            effect = component;
        }else Debug.LogError($"找不到特效: {id}");
        if (playnow)
        {
            effect.PlayTrailEffect(originposition, targetposition, duration);
        }
        else
        {
            onPlayEffect.AddListener(() => effect.PlayTrailEffect(originposition, targetposition, duration));
        }
        return effect;
    }
    /// <summary>
    /// 播放定点特效
    /// <para>playnow : 是否立刻播放</para>
    /// <para>id : 特效ID</para>
    /// <para>target : 目标</para>
    /// <para>duration : 持续时间 , 默认为0.7</para>
    /// <para>Delay : 延迟时间</para>
    /// <para>order : z坐标偏移量</para>
    /// <para>offset : 相对目标的偏移量</para>
    /// </summary>
    public VFXBase PlayPointEffect(bool playnow, string id, GameObject target, float duration = 0.2f, float order =4, float Delay = 0, Vector3 offset = new Vector3())
    {

        VFXBase effect = null;
        Vector3 targetposition = target.transform.position+ new Vector3(0, 0, order) + offset;

        if (ComposDictionary.TryGetValue(id,out VFXComponent Pointcompos))
        {
            VFXComponent composition = Instantiate(Pointcompos, targetposition, Quaternion.identity);
            composition.TotalDelay = Delay;
            activeEffects.Add(composition);
            effect = composition;
        }
        else if (composconfigs.TryGetValue(id, out List<VFXConfig> composconfig))
        {
            VFXComponent component = new VFXComponent();
            component = Instantiate(component, targetposition, Quaternion.identity);
            component.SetConfigs(composconfig);
            component.SetDelay(Delay);
            activeEffects.Add(component);
            effect = component;
        }
        else if (SpotDictionary.TryGetValue(id,out VFXBase PointBase))
        {
            VFXBase component = Instantiate(PointBase, targetposition, Quaternion.identity);
            component.SetDelay(Delay);
            activeEffects.Add(component);
            effect = component;
        }
        else if (configs.TryGetValue(id, out VFXConfig vFXConfig))
        {
            VFXBase component = Instantiate(SpotDictionary[vFXConfig.id], targetposition, Quaternion.identity);
            component.SetConfig(vFXConfig);
            component.SetDelay(Delay);
            activeEffects.Add(component);
            effect = component;
        }
        else Debug.LogError($"找不到特效: {id}");
        if (playnow)
        {
            effect.PlayPointEffect(targetposition, duration);
        }
        else
        {
            onPlayEffect.AddListener(() => effect.PlayPointEffect(targetposition, duration));
        }
        
        return effect;
    }

    #endregion

    #region 私有方法
    private void RemoveEffect(VFXBase effect)
    {
        if (effect != null && activeEffects.Contains(effect))
        {
            activeEffects.Remove(effect);
        }
    }
    private void Update()
    {
        for (int i = 0; i <= activeEffects.Count - 1; i++)
        {
            if (activeEffects[i].IsFinished)
            {
                Destroy(activeEffects[i].gameObject);
                RemoveEffect(activeEffects[i]);
            }
        }
    }
    #endregion
}