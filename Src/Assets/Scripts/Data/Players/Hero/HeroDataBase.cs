using System.Collections.Generic;
using System.IO;
using UnityEngine;

// ???????
public class HeroDataBase : MonoSingleton<HeroDataBase>
{
    public string path;

    // ????
    public Dictionary<string, HeroDefine> HeroDictionary { get; private set; }

    private new void Awake()
    {
        base.Awake();
        LoadingHeroes();
        PrintAllHeroes();
    }

    public void LoadingHeroes()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Hero/Hero.json");
        Debug.Log($"[HeroDataBase] Loading Hero.json from: {path}");

        HeroDictionary = JsonLoader.DeserializeObject<Dictionary<string, HeroDefine>>(path);

        if (HeroDictionary == null || HeroDictionary.Count == 0)
        {
            Debug.LogError("[HeroDataBase] HeroDictionary is NULL or EMPTY!");
        }
        else
        {
            //Debug.Log($"[HeroDataBase] Loaded {HeroDictionary.Count} heroes.");
            foreach (var hero in HeroDictionary)
            {
                //Debug.Log($"[HeroDataBase] HeroID: {hero.Key}, Name: {hero.Value.Name}");
            }
        }

        MyLog.PrintLoadedDictionary(HeroDictionary, "Log/Loading/Heroes.txt");
    }
    // HeroDataBase.cs ??????????
    public void PrintAllHeroes()
    {
        if (HeroDictionary == null || HeroDictionary.Count == 0)
        {
            Debug.LogWarning("[HeroDataBase] ??????");
            return;
        }

        //Debug.Log("=== [HeroDataBase] ????????? ===");
        foreach (var kvp in HeroDictionary)
        {
            HeroDefine hero = kvp.Value;
            /*
            Debug.Log(
                $"HeroID: {hero.ID}\n" +
                $"  name: {hero.Name}\n" +
                $"  description: {hero.Description}\n" +
                $"  maxHP: {hero.MaxHP}\n" +
                $"  skill: {(hero.SkillIDList != null && hero.SkillIDList.Count > 0 ? string.Join(", ", hero.SkillIDList) : "?")}\n"
            );
            */
        }
    }
    public List<HeroDefine> GetAllHeroes()
    {
        if (HeroDictionary == null)
        {
            Debug.LogError("[HeroDataBase] HeroDictionary ??????? LoadingHeroes()");
            return new List<HeroDefine>();
        }

        return new List<HeroDefine>(HeroDictionary.Values);
    }

}
