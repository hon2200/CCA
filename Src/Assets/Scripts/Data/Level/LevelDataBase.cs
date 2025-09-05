using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LevelDataBase : Singleton<LevelDataBase>
{
    public string path;
    public Dictionary<string, LevelDefine> LevelDictionary { get; set; }
    //读入所有玩家
    public void LoadingLevel()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Levels/Level.json");
        LevelDictionary = JsonLoader.DeserializeObject<Dictionary<string, LevelDefine>>(path);
        //打印行动类到日志
        Log.PrintLoadedDictionary(LevelDictionary, "Log/Loading/LevelLog.txt");
    }
}
