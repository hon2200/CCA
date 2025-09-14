using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 特效预设属性
/// <para>name : 用于我们快速查找</para>
/// <para>id : 使用的基础单体组件名</para>
/// <para>duration : 持续时间</para>
/// <para>delay : 延迟 （组合时应用）</para>
/// <para>size : 尺寸</para>
/// <para>size3D : 3D尺寸</para>
/// <para>bursttime : 爆发间隔</para>
/// <para>count : 爆发数量</para>
/// <para>rateOverTime : 速率</para>
/// <para>color : 颜色</para>
/// <para>speedFactor : 速度系数（组合时应用）</para>
/// </summary>
public class VFXConfig
{
    public string name;
    public string id;
    public float duration;
    public float delay;
    public float size;
    public float[] size3D;
    public float[] bursttime;
    public int[] count;
    public int rateOverTime;
    public float[] color;
    public float speedFactor;
}

/// <summary>
/// 特效预设字典
/// </summary>
public class VFXConfigDictionary:Singleton<VFXConfigDictionary>
{
    [Tooltip("配置路径")]
    public string DataPath;

    [Tooltip("单特效配置字典")]
    public Dictionary<string, VFXConfig> config=new Dictionary<string, VFXConfig>();


    /// <summary>
    /// 读取
    /// </summary>
    public void Load()
    {
        DataPath = Path.Combine(Application.dataPath, "Common/Tables/Data/VFX/");
        string json = File.ReadAllText(this.DataPath + "VFXBase.json");
        this.config = JsonConvert.DeserializeObject<Dictionary<string, VFXConfig>>(json);
        VFXManager.Instance.configs = config;
    } 
}