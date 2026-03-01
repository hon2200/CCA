using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class RelicDatabase : MonoSingleton<RelicDatabase>
{
    public string path;
    public Dictionary<string, RelicDefine> RelicDictionary { get; set; }
    public void Awake()
    {
        LoadingRelics();
    }
    //读入所有玩家
    public void LoadingRelics()
    {
        path = Path.Combine(Application.streamingAssetsPath, "Common/Tables/Data/Relic/Relic.json");
        RelicDictionary = JsonLoader.DeserializeObject<Dictionary<string, RelicDefine>>(path);
        //打印行动类到日志
        MyLog.PrintLoadedDictionary(RelicDictionary, "Log/Loading/Relics.txt");
    }
}