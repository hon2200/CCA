using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Holds the raw hero data loaded from Hero.json and enemy data from Enemy.json.
/// </summary>
public class HeroDataBase : MonoSingleton<HeroDataBase>
{
    public Dictionary<string, HeroDefine> HeroDictionary { get; private set; }
    public Dictionary<string, EnemyDefine> EnemyDictionary { get; private set; }

    private void Awake()
    {
        LoadingHeroes();
        LoadingEnemies();
    }

    /// <summary>
    /// Loads Hero.json into HeroDictionary.
    /// </summary>
    public void LoadingHeroes()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/Hero&Enemy/Hero.json");
        HeroDictionary = JsonLoader.DeserializeObject<Dictionary<string, HeroDefine>>(path);

        if (HeroDictionary == null || HeroDictionary.Count == 0)
            Debug.LogError("[HeroDataBase] HeroDictionary is NULL or EMPTY!");
        else
            MyLog.PrintLoadedDictionary(HeroDictionary, "Log/Loading/Heroes.txt");
    }

    /// <summary>
    /// Loads Enemy.json into EnemyDictionary.
    /// </summary>
    public void LoadingEnemies()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/Hero&Enemy/Enemy.json");
        EnemyDictionary = JsonLoader.DeserializeObject<Dictionary<string, EnemyDefine>>(path);

        if (EnemyDictionary == null || EnemyDictionary.Count == 0)
            Debug.LogError("[HeroDataBase] EnemyDictionary is NULL or EMPTY!");
        else
            MyLog.PrintLoadedDictionary(EnemyDictionary, "Log/Loading/Enemies.txt");
    }
}
