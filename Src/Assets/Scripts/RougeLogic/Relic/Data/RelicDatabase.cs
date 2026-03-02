using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class RelicDatabase : MonoSingleton<RelicDatabase>
{
    /// <summary>All registered relic instances (concrete RelicDefine subclasses).</summary>
    public Dictionary<string, RelicDefine> RelicDictionary = new Dictionary<string, RelicDefine>();

    /// <summary>
    /// Load JSON into RelicDatabaseOrigin, then register all concrete relics.
    /// </summary>
    public void LoadingRelics()
    {
        RelicDatabaseOrigin.Instance.LoadingRelics();
    }

    private void Awake()
    {
        LoadingRelics();
        RegisterAllRelics();
    }

    /// <summary>
    /// Scans for all concrete RelicDefine subclasses, creates one instance each, and registers them.
    /// Init() loads data from RelicDatabaseOrigin.RelicDictionary.
    /// </summary>
    private void RegisterAllRelics()
    {
        RelicDictionary.Clear();

        var relicTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(RelicDefine).IsAssignableFrom(t) && t != typeof(RelicDefine));

        foreach (var type in relicTypes)
        {
            try
            {
                RelicDefine relic = Activator.CreateInstance(type) as RelicDefine;
                if (relic != null && !string.IsNullOrEmpty(relic.ID))
                {
                    RelicDictionary[relic.ID] = relic;
                    Debug.Log($"[RelicDatabase] 已注册遗物: {relic.ID} ({type.Name})");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[RelicDatabase] 注册遗物失败: {type.Name}, 错误: {e.Message}");
            }
        }

        Debug.Log($"[RelicDatabase] 总共注册 {RelicDictionary.Count} 个遗物");
    }

    /// <summary>
    /// Get a relic instance by ID.
    /// </summary>
    public RelicDefine GetRelic(string relicID)
    {
        if (RelicDictionary.TryGetValue(relicID, out RelicDefine relic))
            return relic;
        Debug.LogWarning($"[RelicDatabase] 找不到遗物ID: {relicID}");
        return null;
    }
}
