using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Image;

/// <summary>
/// 特效组
/// </summary>
public class VFXComponent : VFXBase
{
    [Tooltip("组合ID")]
    public string CompositionId;
    public VFXComConfigs comConfigs;
    public UnityEvent onPlayEffect = new UnityEvent();

    public float playtime = 0;

    [Tooltip("包含的组件")]
    public List<VFXBase> ChildComponents = new List<VFXBase>();


    public void SetConfigs(VFXComConfigs configs, Vector3 origin, Vector3 target = new Vector3())
    {
        comConfigs = configs;
        ChildComponents.Clear();
        CompositionId = configs.id;
        for (int i = 0; i < configs.components.Length; i++)
        {
            if (VFXDictionary.Instance.configs.TryGetValue(configs.components[i], out VFXConfig vFXConfig))
            {
                VFXBase effect;
                if (VFXManager.Instance.TrailDictionary.TryGetValue(vFXConfig.id, out VFXBase vFX))
                {
                    effect = Instantiate(VFXManager.Instance.TrailDictionary[vFXConfig.id], origin, gameObject.transform.rotation);
                }
                else
                {
                    effect = Instantiate(VFXManager.Instance.SpotDictionary[vFXConfig.id], origin, gameObject.transform.rotation);
                }
                float angle = Quaternion.Angle(Quaternion.LookRotation(target - origin), Quaternion.identity);
                Vector3 rotatedVector = Quaternion.Euler(0, 0, angle) * configs.offects[i];
                effect.gameObject.transform.SetParent(this.transform);
                vFXConfig.duration = configs.duration;
                vFXConfig.delay = configs.delay;
                effect.origin = origin + rotatedVector;
                effect.target = target + rotatedVector;
                effect.SetConfig(vFXConfig);
                onPlayEffect.AddListener(() => effect.PlayEffect());
                ChildComponents.Add(effect);
            }
        }
    }

    public override void SetDuration(float duration)
    {
        foreach (var kv in ChildComponents)
        {
            kv.SetDuration(duration);
        }
    }

    public override void SetDelay(float delay)
    {
        foreach (var kv in ChildComponents)
        {
            kv.SetDelay(delay);
        }
    }


    public override void PlayEffect()
    {
        playtime = Time.time;
        foreach (var kv in ChildComponents)
        {
            kv.PlayEffect();
        }

        onPlayEffect?.Invoke();
        onPlayEffect.RemoveAllListeners();
    }

    public override bool IsFinished()
    {
        return base.IsFinished() || Time.time >= playtime + comConfigs.duration + 1;
    }

}