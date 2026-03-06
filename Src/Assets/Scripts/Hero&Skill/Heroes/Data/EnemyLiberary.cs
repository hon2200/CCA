using AYellowpaper.SerializedCollections;
using UnityEngine;

// Enemy library: index enemies by ID and get EnemyTemplete (prefab/resources) for each enemy.
public class EnemyLiberary : MonoSingleton<EnemyLiberary>
{
    [Tooltip("Path under Resources folder (e.g. 'Scriptables/EnemyScriptables' for Assets/Resources/Scriptables/EnemyScriptables)")]
    [SerializeField] private string _resourcesPath = "Scriptables/EnemyScriptables";

    public SerializedDictionary<string, EnemyTemplete> EnemyDictionary { get; private set; }

    protected override void OnStart()
    {
        LoadAllEnemies();
    }

    /// <summary>
    /// Load all EnemyTemplete ScriptableObjects from the Resources folder and fill EnemyDictionary.
    /// </summary>
    public void LoadAllEnemies()
    {
        EnemyTemplete[] templates = Resources.LoadAll<EnemyTemplete>(_resourcesPath);
        EnemyDictionary = new SerializedDictionary<string, EnemyTemplete>();

        foreach (EnemyTemplete template in templates)
        {
            if (template == null) continue;
            if (string.IsNullOrEmpty(template.ID))
            {
                Debug.LogWarning($"EnemyLiberary: EnemyTemplete '{template.name}' has empty ID, skipping.");
                continue;
            }
            if (EnemyDictionary.ContainsKey(template.ID))
            {
                Debug.LogWarning($"EnemyLiberary: Duplicate ID '{template.ID}' for asset '{template.name}', skipping.");
                continue;
            }
            EnemyDictionary.Add(template.ID, template);
        }

        Debug.Log($"EnemyLiberary: Loaded {EnemyDictionary.Count} enemy(ies) from Resources/{_resourcesPath}.");
    }
}
