using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Displays all relics from RougeManager.rougePlayer.Relics in a line.
/// Updated by RougeManager; also refreshes on Enable and when RefreshDisplay() is called.
/// </summary>
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

        var library = RelicLiberary.Instance;
        if (library?.RelicDictionary == null)
        {
            Debug.LogWarning("[RelicDisplay] RelicLiberary.Instance or RelicDictionary is null. Ensure RelicLiberary is in the scene and RelicDictionary is assigned.");
            return;
        }

        for (int i = 0; i < relics.Count; i++)
        {
            var relic = relics[i];
            if (relic == null) continue;

            if (!library.RelicDictionary.TryGetValue(relic.ID, out var template))
                continue;

            var instance = Instantiate(relicPrefab, container);
            instance.transform.localPosition = new Vector3(i * spacing, 0f, 0f);

            var runtimeRelic = instance.GetComponent<RuntimeRelic>();
            if (runtimeRelic != null)
            {
                runtimeRelic.relicTemplete = template;
                runtimeRelic.relicDefine = relic;
            }

            var relicUI = instance.GetComponent<RelicUI>();
            if (relicUI != null)
                relicUI.Initialize(template, relic);

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
