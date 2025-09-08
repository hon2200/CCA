using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class PlayerSpacingDataBase : MonoSingleton<PlayerSpacingDataBase>
{
    public void Start()
    {
        LoadingPlayers();
    }
    public Dictionary<int, PlayerSpacing> playerSpacingDictionary;
    public void LoadingPlayers()
    {
        string path = Path.Combine(Application.dataPath, "Common/Tables/Data/Space/PlayerSpacing.json");
        playerSpacingDictionary = JsonLoader.DeserializeObject<Dictionary<int, PlayerSpacing>>(path);
        //打印行动类到日志
        MyLog.PrintLoadedDictionary(playerSpacingDictionary, "Log/Loading/PlayerSpacinglog.txt");
    }
}
