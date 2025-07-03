using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

class ActionPhase : Singleton<ActionPhase>, Phase
{
    public void OnEnteringPhase()
    {
        foreach(var player in PlayerManager.Instance.Players)
        {
            //保存每个玩家的状态，在ActionPhase无论Player还是AI都会对其进行非最终的改动
            player.Value.status.SaveStatus();
        }
        CardSelectionManager.Instance.Enable();
        //反正AI是要行动的对吧，AI就先行动了。这回不玩赖的了
        AIMoveTogether();
        //进入选择行动阶段，等待玩家准备
        foreach (var player in PlayerManager.Instance.Players)
        {
            player.Value.isReady.Cancel();
        }
    }
    public void OnExitingPhase()
    {
        foreach (var player in PlayerManager.Instance.Players)
        {
            //读取每个玩家的状态，在ActionPhase无论Player还是AI都会对其进行非最终的改动
            player.Value.status.LoadStatus();
        }
        CardSelectionManager.Instance.Disable();
    }
    public void AIMoveTogether()
    {
        foreach(var player in PlayerManager.Instance.Players.Values)
        {
            if (player.playerType == PlayerType.AI)
            {
                AILogic aiLogic = new(player);
                aiLogic.AIMove();
            }
        }
    }

    #region Abandoned
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
                player.Value.action.ReadinMove(move.Item1, move.Item2, "Debug");
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
    #endregion
}
