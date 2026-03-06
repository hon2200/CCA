using AYellowpaper.SerializedCollections;
using UnityEngine;

// Hero library: index heroes by ID and get HeroTemplete (prefab/resources) for each hero.
public class HeroLiberary : MonoSingleton<HeroLiberary>
{
    [Tooltip("Path under Resources folder (e.g. 'Scriptables/HeroScriptables' for Assets/Resources/Scriptables/HeroScriptables)")]
    [SerializeField] private string _resourcesPath = "Scriptables/HeroScriptables";

    public SerializedDictionary<string, HeroTemplete> HeroDictionary { get; private set; }

    protected override void OnStart()
    {
        LoadAllHeroes();
    }

    /// <summary>
    /// Load all HeroTemplete ScriptableObjects from the Resources folder and fill HeroDictionary.
    /// </summary>
    public void LoadAllHeroes()
    {
        HeroTemplete[] templates = Resources.LoadAll<HeroTemplete>(_resourcesPath);
        HeroDictionary = new SerializedDictionary<string, HeroTemplete>();

        foreach (HeroTemplete template in templates)
        {
            if (template == null) continue;
            if (string.IsNullOrEmpty(template.ID))
            {
                Debug.LogWarning($"HeroLiberary: HeroTemplete '{template.name}' has empty ID, skipping.");
                continue;
            }
            if (HeroDictionary.ContainsKey(template.ID))
            {
                Debug.LogWarning($"HeroLiberary: Duplicate ID '{template.ID}' for asset '{template.name}', skipping.");
                continue;
            }
            HeroDictionary.Add(template.ID, template);
        }

        Debug.Log($"HeroLiberary: Loaded {HeroDictionary.Count} hero(s) from Resources/{_resourcesPath}.");
    }
}
