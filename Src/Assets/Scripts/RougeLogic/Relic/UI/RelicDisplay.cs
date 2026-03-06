using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Displays all relics from RougeManager.rougePlayer.Relics in a line.
/// Updated by RougeManager; also refreshes on Enable and when RefreshDisplay() is called.
/// </summary>
/// 这个可以优化！只需要更新新遗物
public class RelicDisplay : MonoSingleton<RelicDisplay>
{
    [SerializeField] private GameObject relicPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private float spacing = 1.5f;

    private readonly List<GameObject> _spawnedRelics = new List<GameObject>();

    private void OnEnable()
    {
        RefreshDisplay();
    }

    /// <summary>
    /// Rebuild the line of relic UI from RougeManager.rougePlayer.Relics.
    /// </summary>
    public void RefreshDisplay()
    {
        if (container == null || relicPrefab == null)
        {
            if (container == null)
                Debug.LogWarning("[RelicDisplay] Container is not assigned. Assign a Transform in the Inspector.");
            if (relicPrefab == null)
                Debug.LogWarning("[RelicDisplay] Relic Prefab is not assigned. Assign a prefab with RuntimeRelic and RelicUI.");
            return;
        }

        foreach (var go in _spawnedRelics)
        {
            if (go != null)
                Destroy(go);
        }
        _spawnedRelics.Clear();

        var relics = RougeManager.Instance?.rougePlayer?.Relics;
        if (relics == null || relics.Count == 0) return;

        for (int i = 0; i < relics.Count; i++)
        {
            var relic = relics[i];
            if (relic == null) continue;

            var instance = Instantiate(relicPrefab, container);
            instance.transform.localPosition = new Vector3(i * spacing, 0f, 0f);

            var runtimeRelic = instance.GetComponent<RuntimeRelic>();
            if (runtimeRelic != null)
            {
                runtimeRelic.relicDefine = relic;
                RelicLiberary.Instance.RelicDictionary.TryGetValue(relic.ID, out var relicTemplete);
                if (relicTemplete == null)
                {
                    RelicLiberary.Instance.RelicDictionary.TryGetValue("Default", out var defaultTemplete);
                    runtimeRelic.relicTemplete = defaultTemplete;
                }
                runtimeRelic.relicTemplete = relicTemplete;
            }
            _spawnedRelics.Add(instance);
        }
    }

    private int GetDisplayedCount()
    {
        int n = 0;
        foreach (var go in _spawnedRelics)
        {
            if (go != null) n++;
        }
        return n;
    }
}
