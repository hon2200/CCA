using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    public GameObject playerPrefab;
    public Dictionary<int, Player> Players;
    public void CreatingPlayers_Debug()
    {
        Players = new Dictionary<int, Player>();
        //创建人类玩家
        var newHumanPlayer = CreatePlayer_Debug(1, PlayerType.Human);
        //加入玩家
        Players.Add(1, newHumanPlayer);
        //创建AI玩家
        for (int i = 2; i <= 5; i++)
        {
            var newPlayer = CreatePlayer_Debug(i, PlayerType.AI);
            //加入玩家
            Players.Add(i, newPlayer);
        }
        InitializePlayerSpace(5);
        //Log.PrintLoadedDictionary(Players, "Log/Loading/PlayerTable_Debug.txt");
    }
    //创建HP=5的用于测试的玩家
    private Player CreatePlayer_Debug(int ID_inGame,PlayerType playerType)
    {
        PlayerDefine playerDefine = new("Debug_Player", playerType, 5, new PlayerResource(0, 0, 0));
        PlayerStatus playerStatus = new(playerDefine);
        PlayerAction playerAction = new();
        //创建玩家物体
        var newPlayerObject = Instantiate(playerPrefab,this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //初始化玩家脚本
        var newPlayer = newPlayerObject.GetComponent<Player>();
        newPlayer.Initialize(ID_inGame, playerStatus, playerAction);
        InitializeUIText(newPlayer);
        return newPlayer;
    }
    private void InitializeUIText(Player newPlayer)
    {
        newPlayer.playerUIText.Initialize();
    }
    private void InitializePlayerSpace(int playerCount)
    {
        // 获取位置配置
        //需要维护者两个spacing（x和y坐标）的并行列表长度一致，未来考虑改进
        if (!PlayerSpacingDataBase.Instance.playerSpacingDictionary.TryGetValue(playerCount, out var spacingData))
        {
            Debug.LogError($"No spacing data found for player count: {playerCount}");
            return;
        }

        // 创建位置列表并复制（避免修改原始数据）
        var availablePositions = new List<Vector2Int>();
        for (int i = 0; i < spacingData.Player_X.Count; i++)
        {
            availablePositions.Add(new Vector2Int(spacingData.Player_X[i], spacingData.Player_Y[i]));
        }

        // 先处理人类玩家（确保人类玩家有固定位置）
        foreach (var player in Players.Values)
        {
            if (player.status.playerDefine.Type == PlayerType.Human)
            {
                // 人类玩家固定位置
                player.transform.localPosition = new Vector3(0, -300, 0);
            }
        }

        // 然后处理AI玩家
        foreach (var player in Players.Values)
        {
            if (player.status.playerDefine.Type == PlayerType.AI && availablePositions.Count > 0)
            {
                // 使用并移除第一个可用位置
                var pos = availablePositions[0];
                player.transform.localPosition = new Vector3(pos.x, pos.y, 0);
                availablePositions.RemoveAt(0);
            }
        }
    }
}
