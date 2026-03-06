using AYellowpaper.SerializedCollections;
using UnityEngine;

public class RelicLiberary : MonoSingleton<RelicLiberary>
{
    [Tooltip("Path under Resources folder (e.g. 'Scriptables/RelicScriptables' for Assets/Resources/Scriptables/RelicScriptables)")]
    [SerializeField] private string _resourcesPath = "Scriptables/RelicScriptables";

    public SerializedDictionary<string, RelicTemplete> RelicDictionary { get; private set; }

    protected override void OnStart()
    {
        LoadAllRelics();
    }

    /// <summary>
    /// Load all RelicTemplete ScriptableObjects from the Resources folder and fill RelicDictionary.
    /// </summary>
    public void LoadAllRelics()
    {
        RelicTemplete[] templates = Resources.LoadAll<RelicTemplete>(_resourcesPath);
        RelicDictionary = new SerializedDictionary<string, RelicTemplete>();

        foreach (RelicTemplete template in templates)
        {
            if (template == null) continue;
            if (string.IsNullOrEmpty(template.ID))
            {
                Debug.LogWarning($"RelicLiberary: RelicTemplete '{template.name}' has empty ID, skipping.");
                continue;
            }
            if (RelicDictionary.ContainsKey(template.ID))
            {
                Debug.LogWarning($"RelicLiberary: Duplicate ID '{template.ID}' for asset '{template.name}', skipping.");
                continue;
            }
            RelicDictionary.Add(template.ID, template);
        }

        Debug.Log($"RelicLiberary: Loaded {RelicDictionary.Count} relic(s) from Resources/{_resourcesPath}.");
    }
}

