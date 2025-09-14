using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.ParticleSystem;

/// <summary>
/// 特效基类
/// </summary>
public class VFXBase : MonoBehaviour
{

    [Tooltip("是否已完成")]
    public bool IsFinished = false;

    [Tooltip("粒子系统")]
    public ParticleSystem particleSystem;

    [Tooltip("预定配置")]
    public VFXConfig currentConfig;

    public virtual void Init()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// 应用特效配置
    /// </summary>
    public virtual void SetConfig(VFXConfig config)
    {
        currentConfig = config;
        Init();
        if (particleSystem == null|| currentConfig == null) return;
        particleSystem.Stop();
        if (config.duration >0) SetDuration(config.duration);
        if (config.delay > 0) SetDelay(config.delay);
        if (config.size > 0) SetSize(config.size);
        if (config.size3D != null && config.size3D.Length >= 3) Set3DSize(config.size3D[0], config.size3D[1], config.size3D[2]);
        if (config.count!=null && config.count.Length >=1) SetCount(config.bursttime,config.count);
        if (config.rateOverTime > 0) SetRateOverTime(config.rateOverTime);
        if (config.color != null && config.color.Length >= 4)
        {
            SetColor(new Color(
                config.color[0] / 255f,
                config.color[1] / 255f,
                config.color[2] / 255f,
                config.color[3] / 255f
            ));
        }
    }

    /// <summary>
    /// 播放轨迹特效
    /// </summary>
    public virtual void PlayTrailEffect(Vector3 start, Vector3 end, float duration)
    {

        transform.position = start;
        if (particleSystem == null) return;            
        float distance = Vector3.Distance(start, end);            
        var main = particleSystem.main;            
        main.startSpeed = distance / duration;            
        main.startLifetime = duration;           
        main.gravityModifier = 0f;            
        main.simulationSpace = ParticleSystemSimulationSpace.World;            
        transform.rotation = Quaternion.LookRotation(end - start);            
        particleSystem.Play();
        StartCoroutine(DestroyAfterDuration(duration));
    }

    /// <summary>
    /// 播放定点特效
    /// </summary>
    public virtual void PlayPointEffect(Vector3 position, float duration)
    {
        transform.position = position;
        if (particleSystem == null) return;
        var main = particleSystem.main;
        main.startLifetime = duration;
        particleSystem.Play();
        IsFinished = false;
        StartCoroutine(DestroyAfterDuration(duration));
    }



    #region 内置设置方法
    public virtual void SetDuration(float duration)
    {
        if (particleSystem == null) return;
        var main = particleSystem.main;
        main.duration = duration;
    }
    public virtual void SetDelay(float delay)
    {
        if (particleSystem == null) return;
        var main = particleSystem.main;
        main.startDelay = delay;
        particleSystem.Stop();
    }
    public virtual void SetSize(float size)
    {
        if (particleSystem == null) return;
        var main = particleSystem.main;
        if (size > 0) main.startSize = size;
    }
    public virtual void Set3DSize(float x,float y,float z)
    {
        if (particleSystem != null)
        {
            var main = particleSystem.main;
            main.startSize3D = true;
            main.startSizeX = x;
            main.startSizeY = y;
            main.startSizeZ = z;
        }
    }
    public virtual void SetCount(float[] bursttime ,int[] count)
    {
        if (particleSystem == null|| count == null) return;
        if (bursttime == null&& count.Length == 1) bursttime = new float[] { 0};
        if (bursttime.Length != count.Length) return;
        var emission = particleSystem.emission;
        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[bursttime.Length];
        for (int i = 0; i < bursttime.Length; i++)
        {
            bursts[i] = new ParticleSystem.Burst(bursttime[i], count[i]);
        }
        emission.SetBursts(bursts);
    }
    public virtual void SetRateOverTime(int rateOverTime = 0)
    {
        if (particleSystem == null) return;
        var emission = particleSystem.emission;
        if (rateOverTime>0) emission.rateOverTime = rateOverTime;
    }
    public virtual void SetColor(Color color)
    {
        if (particleSystem == null) return;
        var main = particleSystem.main;
        if (color!=null) main.startColor = color;
    }
    #endregion

    /// <summary>
    /// 设置速度
    /// </summary>
    public virtual void SetSpeed(float speed)
    {
        if (particleSystem == null) return;
        var main = particleSystem.main;
        if (speed>0) main.startSpeed = speed;
    }
    private IEnumerator DestroyAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        while (particleSystem.isPlaying == true)
        {
            yield return new WaitForSeconds(0.5f);
        }
        IsFinished = true;
    }
}