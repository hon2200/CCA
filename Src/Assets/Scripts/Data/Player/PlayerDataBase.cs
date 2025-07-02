using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerDataBase : Singleton<PlayerDataBase>
{
    public string path;
    public Dictionary<int, PlayerDefine> playerDictionary;
    // 玩家字典，包含所有玩家数据库
    public Dictionary<int, PlayerDefine> PlayerDictionary { get; set; }
    //读入所有玩家
    public void LoadingPlayers()
    {
        path = Path.Combine(Application.dataPath, "Common/Tables/Data/Player/Player.json");
        playerDictionary = JsonLoader.DeserializeObject<Dictionary<int, PlayerDefine>>(path);
        //打印行动类到日志
        Log.PrintLoadedDictionary(PlayerDictionary,"Log/Loading/Playerlog.txt");
    }
}
