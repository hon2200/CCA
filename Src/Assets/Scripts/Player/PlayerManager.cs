using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//这两天写一个通过UI创建英雄的函数
public class PlayerManager : MonoSingleton<PlayerManager>
{
    public int AlivePlayerNumber { get; set; }
    //拥有情感的AI的预制体（也就是挂载的脚本变了一下）
    public GameObject AIPrefab;
    public GameObject HumanPrefab;
    public Dictionary<int, Player> Players;

    public List<Player> GetAlivePlayers()
    {
        List<Player> liveones = new();
        foreach(var player in Players.Values)
        {
            if(player.status.life.Value != LifeStatus.Death)
                liveones.Add(player);
        }
        return liveones;
    }

    #region AI Things
    public void CreateCurrentLevelWave()
    {
        Debug.Log("加载关卡" + LevelManager.Instance.Level.ID + "第" + LevelManager.Instance.Level.Wave + "波");
        LevelDataBase.Instance.LevelDictionary.TryGetValue(LevelManager.Instance.Level.ID, out var levelData);
        CreatingPlayers_BasedOnLevels(levelData, LevelManager.Instance.Level.Wave);
    }
    //Wave是从第0波开始计数的
    public void CreatingPlayers_BasedOnLevels(LevelDefine levelDefine, int Wave)
    {
        int friendCount = 0, enemyCount = 0, remains = 0;
        if (Wave == 0)
        {
            //清理所有人
            if (Players != null)
            {
                List<int> playersToRemove = new List<int>();
                foreach (var player in Players.Values)
                {
                    playersToRemove.Add(player.ID_inGame);
                }
                // 移除
                foreach (int playerId in playersToRemove)
                {
                    if (Players.TryGetValue(playerId, out var playerToDestroy))
                    {
                        Players.Remove(playerId);
                        Destroy(playerToDestroy.gameObject); // 假设player是Component
                                                             // 或者如果是GameObject：Destroy(playerToDestroy);
                    }
                }
            }
            //先新建数组
            Players = new Dictionary<int, Player>();
            //创建人类
            Player newPlayerH = CreateHuman_BasedOnLevel(1, levelDefine);
            Players.Add(1, newPlayerH);
        }

        else
        {
            //清理死人清点活人
            // 先收集需要移除的玩家
            List<int> playersToRemove = new List<int>();

            foreach (var player in Players.Values)
            {
                if (player.status.life.Value == LifeStatus.Death)
                {
                    playersToRemove.Add(player.ID_inGame);
                }
                else if (player.playerType != PlayerType.Human)
                {
                    remains++;
                }
            }

            // 移除死亡的玩家
            foreach (int playerId in playersToRemove)
            {
                if (Players.TryGetValue(playerId, out var playerToDestroy))
                {
                    Players.Remove(playerId);
                    Destroy(playerToDestroy.gameObject); // 假设player是Component
                                                         // 或者如果是GameObject：Destroy(playerToDestroy);
                }
            }
        }
        //如果友方还有足够的波数的话
        if (levelDefine.FriendList.Count > Wave)
        {
            friendCount = levelDefine.FriendList[Wave].Count;
            for (int j = 0; j < friendCount; j++)
            {
                AIDataBase.Instance.AIDictionary.TryGetValue(levelDefine.FriendList[Wave][j], out var AI);
                if (AI != null)
                {
                    Player newPlayer = CreateAI(j + remains + 2, AI, true);
                    Players.Add(j + remains + 2, newPlayer);
                }

                else
                    Debug.Assert(false, "Can't find AI" + levelDefine.FriendList[Wave][j]);
            }
        }
        if (levelDefine.EnemyList.Count > Wave)
        {
            enemyCount = levelDefine.EnemyList[Wave].Count;
            for (int i = 0; i < enemyCount; i++)
            {
                AIDataBase.Instance.AIDictionary.TryGetValue(levelDefine.EnemyList[Wave][i], out var AI);
                if (AI != null)
                {
                    Player newPlayer = CreateAI(i + remains + friendCount + 2, AI, false);
                    Players.Add(i + remains + friendCount + 2, newPlayer);
                }

                
                else
                    Debug.Assert(false, "Can't find AI" + levelDefine.EnemyList[Wave][i]);
            }
        }
        int totalNumber = 1 + friendCount + enemyCount + remains;
        InitializePlayerSpace(totalNumber);
        AlivePlayerNumber = totalNumber;
        //MyLog.PrintLoadedDictionary(Players, "MyLog/Loading/PlayerTable_Debug.txt");

    }
    private Player CreateAI(int ID_inGame, AIDefine aIDefine, bool isFriend)
    {
        //创建玩家物体
        var newPlayerObject = Instantiate(AIPrefab, this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //初始化玩家脚本
        var newPlayer = newPlayerObject.GetComponent<AIPlayer>();
        newPlayer.Initialize(ID_inGame, aIDefine, isFriend);
        return newPlayer;
    }
    private Player CreateHuman_BasedOnLevel(int ID_inGame,LevelDefine level)
    {
        //创建玩家物体
        var newPlayerObject = Instantiate(HumanPrefab, this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //初始化玩家脚本
        var newPlayer = newPlayerObject.GetComponent<HumanPlayer>();
        newPlayer.InitializePlayer(ID_inGame, level);
        return newPlayer;
    }
    #endregion

    #region Heroes Things
    public void CreatingPlayers_BasedOnGameSetting_Heroes()
    {
        //没有从Initial Setting进入
        if (GameSetting.Instance == null)
        {
            return;
        }
        Players = new Dictionary<int, Player>();
        List<HeroDefine> heroDefines = new();

        foreach (var heroID in GameSetting.Instance.HeroIDDictionary)
        {
            HeroDataBase.Instance.HeroDictionary.TryGetValue(heroID, out var heroDefine);
            if (heroDefine != null)
                heroDefines.Add(heroDefine);
            else
                Debug.Assert(false, "Can't fine Hero");
        }
        int totalNumber = heroDefines.Count;
        //创建人类玩家
        var newHumanPlayer = CreateHumanHero(1, heroDefines[0]);
        //加入玩家
        Players.Add(1, newHumanPlayer);
        //创建AI玩家
        for (int i = 2; i <= totalNumber; i++)
        {
            var newPlayer = CreateAIHero(i, heroDefines[i - 1]);
            //加入玩家
            Players.Add(i, newPlayer);
        }
        InitializePlayerSpace(totalNumber);
        AlivePlayerNumber = totalNumber;
        //MyLog.PrintLoadedDictionary(Players, "MyLog/Loading/PlayerTable_Debug.txt");
    }
    private Player CreateAIHero(int ID_inGame, HeroDefine heroDefine)
    {
        //创建玩家物体
        var newPlayerObject = Instantiate(AIPrefab, this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //初始化玩家脚本
        var newPlayer = newPlayerObject.GetComponent<AIPlayer>();
        newPlayer.Initialize(ID_inGame, heroDefine);
        return newPlayer;
    }
    private Player CreateHumanHero(int ID_inGame, HeroDefine heroDefine)
    {
        //创建玩家物体
        var newPlayerObject = Instantiate(HumanPrefab, this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //初始化玩家脚本
        var newPlayer = newPlayerObject.GetComponent<HumanPlayer>();
        newPlayer.InitializePlayer(ID_inGame, heroDefine);
        return newPlayer;
    }
    #endregion
    #region Initialization Things
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
        var availablePositions = new List<Vector2>();
        for (int i = 0; i < spacingData.Player_X.Count; i++)
        {
            availablePositions.Add(new Vector2(spacingData.Player_X[i], spacingData.Player_Y[i]));
        }

        // 先处理人类玩家（确保人类玩家有固定位置）
        foreach (var player in Players.Values)
        {
            if (player.playerType == PlayerType.Human)
            {
                // 人类玩家固定位置
                player.transform.localPosition = new Vector3(8, -5, 1);

            }
        }

        // 然后处理AI玩家
        foreach (var player in Players.Values)
        {
            if (player.playerType == PlayerType.AI && availablePositions.Count > 0)
            {
                // 使用并移除第一个可用位置
                var pos = availablePositions[0];
                player.transform.localPosition = new Vector3(pos.x, pos.y, 1);
                availablePositions.RemoveAt(0);
            }
        }
    }
    #endregion
}
