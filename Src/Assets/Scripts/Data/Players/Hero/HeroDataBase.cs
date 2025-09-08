using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//DataBase很多，但是代码基本一样，后续可以简化
public class HeroDataBase : MonoSingleton<HeroDataBase>
{
    public string path;
    // 玩家字典，包含所有玩家数据库
    public Dictionary<string, HeroDefine> HeroDictionary { get; set; }
    private new void Awake()
    {
        base.Awake();
        LoadingHeroes();
    }
    //读入所有玩家
    public void LoadingHeroes()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Hero/Hero.json");
        HeroDictionary = JsonLoader.DeserializeObject<Dictionary<string, HeroDefine>>(path);
        //打印行动类到日志
        MyLog.PrintLoadedDictionary(HeroDictionary,"Log/Loading/Heroes.txt");
    }
}
