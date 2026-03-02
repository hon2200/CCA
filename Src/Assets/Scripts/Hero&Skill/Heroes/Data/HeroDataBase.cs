using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Holds the raw hero data loaded from Hero.json.
/// </summary>
public class HeroDataBase : MonoSingleton<HeroDataBase>
{
    public Dictionary<string, HeroDefine> HeroDictionary { get; private set; }

    private void Awake()
    {
        LoadingHeroes();
    }

    /// <summary>
    /// Loads Hero.json into HeroDictionary.
    /// </summary>
    public void LoadingHeroes()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/Hero/Hero.json");
        HeroDictionary = JsonLoader.DeserializeObject<Dictionary<string, HeroDefine>>(path);

        if (HeroDictionary == null || HeroDictionary.Count == 0)
            Debug.LogError("[HeroDataBase] HeroDictionary is NULL or EMPTY!");
        else
            MyLog.PrintLoadedDictionary(HeroDictionary, "Log/Loading/Heroes.txt");
    }
}
