using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

class ActionPhase:Singleton<ActionPhase>
{
    //测试使用的输入端，通过Json文件自由调控所有玩家的输入
    //这里集合的功能：读入行动，读入历史行动，打印行动结果，考虑使用委托？
    public void ReadinAllActs_Debug(string path = "Common/Data/ReadinMove_Debug/MovesData.json")
    {
        string combinedPath = Path.Combine(Application.dataPath, path);
        Dictionary<int, List<(string, int)>> Movedict
            = JsonLoader.DeserializeObject<Dictionary<int, List<(string, int)>>>(combinedPath, true);
        foreach (var player in PlayerManager.Instance.Players)
        {
            Movedict.TryGetValue(player.Key, out var moves);
            foreach (var move in moves)
            {
                player.Value.action.ReadinMove(move.Item1, move.Item2);
            }
        }
        if (BattleManager.Instance.Turn.Value == 1)
            Log.PrintLoadedDictionary(Movedict, "Log/InGame/Action_Phase.txt");
        else
            Log.PrintLoadedDictionary(Movedict, "Log/InGame/Action_Phase.txt", false);
    }
    public void ReadinOnlyPlayerActs_Debug()
    {
    }
}
