using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

/// <summary>
/// 特效基类
/// </summary>
public class VFXBase : MonoBehaviour
{

    [Tooltip("是否已完成")]
    public bool IsFinished = false;

    [Tooltip("粒子系统")]
    public ParticleSystem PSystem;

    [Tooltip("预定配置")]
    public VFXConfig currentConfig;

    public void Update()
    {
    }

    public virtual void Init()
    {
        PSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// 应用特效配置
    /// </summary>
    public virtual void SetConfig(VFXConfig config)
    {
        currentConfig = config;
        Init();
        if (PSystem == null|| currentConfig == null) return;
        PSystem.Stop();
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
    public virtual void PlayTrailEffect(Vector3 origin, Vector3 target)
    {
        var ps = GetComponent<ParticleSystem>();
        if (ps == null)
        {
            Debug.LogWarning("No ParticleSystem found on VFXBase!");
            return;
        }


        transform.position = origin;
        transform.rotation = Quaternion.LookRotation(target - origin);
        // Project onto XY plane
        origin.z = 0;
        target.z = 0;

        // Compute direction & distance
        Vector3 dir = (target - origin).normalized;
        float distance = Vector3.Distance(origin, target);

        // Orient system to point toward target
        

        // Get particle speed (assuming constant startSpeed)
        var main = ps.main;
        float startSpeed = main.startSpeed.constantMax;
        float travelTime = distance / startSpeed;

        // Play system
        ps.Play();

        // Destroy after travel time + lifetime buffer
        float destroyDelay = travelTime;

        Destroy(gameObject, destroyDelay);
    }

    /// <summary>
    /// 播放定点特效
    /// </summary>
    public virtual void PlayPointEffect(Vector3 position, float duration = 10)
    {
        transform.position = position;

        StartCoroutine(DestroyAfterDuration(duration));
    }



    #region 内置设置方法
    public virtual void SetDuration(float duration)
    {
        if (PSystem == null) return;
        var main = PSystem.main;
        main.duration = duration;
    }
    public virtual void SetDelay(float delay)
    {
        if (PSystem == null) return;
        var main = PSystem.main;
        main.startDelay = delay;
        PSystem.Stop();
    }
    public virtual void SetSize(float size)
    {
        if (PSystem == null) return;
        var main = PSystem.main;
        if (size > 0) main.startSize = size;
    }
    public virtual void Set3DSize(float x,float y,float z)
    {
        if (PSystem != null)
        {
            var main = PSystem.main;
            main.startSize3D = true;
            main.startSizeX = x;
            main.startSizeY = y;
            main.startSizeZ = z;
        }
    }
    public virtual void SetCount(float[] bursttime ,int[] count)
    {
        if (PSystem == null|| count == null) return;
        if (bursttime == null&& count.Length == 1) bursttime = new float[] { 0};
        if (bursttime.Length != count.Length) return;
        var emission = PSystem.emission;
        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[bursttime.Length];
        for (int i = 0; i < bursttime.Length; i++)
        {
            bursts[i] = new ParticleSystem.Burst(bursttime[i], count[i]);
        }
        emission.SetBursts(bursts);
    }
    public virtual void SetRateOverTime(int rateOverTime = 0)
    {
        if (PSystem == null) return;
        var emission = PSystem.emission;
        if (rateOverTime>0) emission.rateOverTime = rateOverTime;
    }
    public virtual void SetColor(Color color)
    {
        if (PSystem == null) return;
        var main = PSystem.main;
        if (color!=null) main.startColor = color;
    }
    #endregion

    /// <summary>
    /// 设置速度
    /// </summary>
    public virtual void SetSpeed(float speed)
    {
        if (PSystem == null) return;
        var main = PSystem.main;
        if (speed>0) main.startSpeed = speed;
    }
    private IEnumerator DestroyAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        IsFinished = true;
    }
}