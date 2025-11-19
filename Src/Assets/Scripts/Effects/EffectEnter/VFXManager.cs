using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting.FullSerializer;
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
    /// 播放预制特效
    /// <para>playnow : 是否立刻播放</para>
    /// <para>id : 特效ID</para>
    /// <para>start : 起点</para>
    /// <para>end : 终点</para>
    /// </summary>
    public VFXBase PlayEffect(bool playnow, string id, GameObject origin, GameObject target = null)
    {
        VFXBase effect = null;
        Vector3 targetposition = target != null ? target.transform.position : Vector3.zero;
        Vector3 originposition = origin.transform.position;
        if (VFXDictionary.Instance.ComConfigs.TryGetValue(id, out VFXComConfigs composconfig))
        {
            GameObject componentObject = new GameObject(composconfig.id);
            componentObject.transform.position = originposition;
            componentObject.transform.rotation = Quaternion.identity;
            VFXComponent component = componentObject.AddComponent<VFXComponent>();
            component.SetConfigs(composconfig, originposition, targetposition);
            activeEffects.Add(component);
            effect = component;
            if (playnow) { component.PlayEffect(); return component; }
        }
        else if (VFXDictionary.Instance.configs.TryGetValue(id, out VFXConfig vFXConfig))
        {
            VFXBase component;
            if (target == null) { component = Instantiate(SpotDictionary[vFXConfig.id], targetposition, SpotDictionary[vFXConfig.id].gameObject.transform.rotation); }
            else { component = Instantiate(TrailDictionary[vFXConfig.id], originposition, Quaternion.LookRotation(targetposition - originposition)); }
            component.origin = originposition;
            component.target = targetposition;
            component.SetConfig(vFXConfig);
            activeEffects.Add(component);
            effect = component;
            if (playnow) { effect.PlayEffect(); return effect; }
        }
        else { Debug.LogError($"找不到特效: {id}"); return null; }

        onPlayEffect.AddListener(() => effect.PlayEffect());

        return effect;
    }
    /// <summary>
    /// 播放特效
    /// <para>playnow : 是否立刻播放</para>
    /// <para>id : 特效ID</para>
    /// <para>start : 起点</para>
    /// <para>end : 终点</para>
    /// <para>duration : 持续时间 , 默认为0.7</para>
    /// <para>Delay : 延迟时间</para>
    /// <para>offset : 相对目标的偏移量</para>
    /// </summary>
    public VFXBase PlayTrailEffect(bool playnow, string id, GameObject origin, GameObject target, float duration = 0.7f, float Delay = 0, Vector3 offset = new Vector3())
    {

        VFXBase effect = PlayEffect(false, id, origin, target);
        if (effect == null) return effect;
        VFXComponent vFX = effect.gameObject.GetComponent<VFXComponent>();
        effect.gameObject.transform.position += offset;
        if (vFX != null)
        {
            vFX.SetDuration(duration);
            vFX.SetDelay(Delay);
            if (playnow) vFX.PlayEffect();
        }
        else
        {
            effect.SetDuration(duration);
            effect.SetDelay(Delay);
            if (playnow) effect.PlayEffect();
        }
        return effect;
    }

    public VFXBase PlayPointEffect(bool playnow, string id, GameObject origin, float duration = 0.7f, float Delay = 0, Vector3 offset = new Vector3())
    {

        VFXBase effect = PlayEffect(false, id, origin, null);
        if (effect == null) return effect;
        VFXComponent vFX = effect.gameObject.GetComponent<VFXComponent>();
        effect.gameObject.transform.position += offset;
        if (vFX != null)
        {
            vFX.SetDuration(duration);
            vFX.SetDelay(Delay);
            if (playnow) vFX.PlayEffect();
        }
        else
        {
            effect.SetDuration(duration);
            effect.SetDelay(Delay);
            if (playnow) effect.PlayEffect();
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
            if (activeEffects[i].IsFinished())
            {
                activeEffects[i].Destroy();
                RemoveEffect(activeEffects[i]);
            }
        }
    }
    #endregion
}