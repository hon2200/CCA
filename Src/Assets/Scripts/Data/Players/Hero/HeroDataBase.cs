using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HeroDataBase : Singleton<HeroDataBase>
{
    public string path;
    // 玩家字典，包含所有玩家数据库
    public Dictionary<int, HeroDefine> HeroDictionary { get; set; }
    //读入所有玩家
    public void LoadingHeroes()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Hero/Hero.json");
        HeroDictionary = JsonLoader.DeserializeObject<Dictionary<int, HeroDefine>>(path);
        //打印行动类到日志
        Log.PrintLoadedDictionary(HeroDictionary,"Log/Loading/Heroes.txt");
    }
}
