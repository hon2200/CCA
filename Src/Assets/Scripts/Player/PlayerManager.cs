using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;



//这两天写一个通过UI创建英雄的函数
public class PlayerManager : MonoSingleton<PlayerManager>
{
    public int AlivePlayerNumber;
    public GameObject playerPrefab;
    //拥有情感的AI的预制体（也就是挂载的脚本变了一下）
    public GameObject AIPrefab;
    public Dictionary<int, Player> Players;
    #region AI Things
    public void CreatingPlayers_BasedOnLevels()
    {
    }

    private Player CreateAI(int ID_inGame, AIDefine aIDefine)
    {
        //创建玩家物体
        var newPlayerObject = Instantiate(playerPrefab, this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //初始化玩家脚本
        var newPlayer = newPlayerObject.GetComponent<AIPlayer>();
        newPlayer.Initialize(ID_inGame, aIDefine);
        InitializeUIText(newPlayer);
        InitializePlayerEffectController(newPlayer);
        return newPlayer;
    }

    private Player CreateHuman_BasedOnLevel(int ID_inGame,int MaxHP)
    {
        //创建玩家物体
        var newPlayerObject = Instantiate(playerPrefab, this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //初始化玩家脚本
        var newPlayer = newPlayerObject.GetComponent<Player>();
        newPlayer.Initialize(ID_inGame, PlayerType.Human, new("Blank", MaxHP));
        InitializeUIText(newPlayer);
        InitializePlayerEffectController(newPlayer);
        return newPlayer;
    }
    #endregion

    #region Heroes Things
    public void CreatingPlayers_BasedOnGameSetting_Heroes()
    {
        Players = new Dictionary<int, Player>();
        List<HeroDefine> heroDefines = new();
        //没有从Initial Setting进入
        if(GameSetting.Instance == null)
        {
            CreateFiveBlank();
        }
        else
        {
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
            var newHumanPlayer = CreateHero(1, PlayerType.Human, heroDefines[0]);
            //加入玩家
            Players.Add(1, newHumanPlayer);
            //创建AI玩家
            for (int i = 2; i <= totalNumber; i++)
            {
                var newPlayer = CreateHero(i, PlayerType.AI, heroDefines[i - 1]);
                //加入玩家
                Players.Add(i, newPlayer);
            }
            InitializePlayerSpace(totalNumber);
            AlivePlayerNumber = totalNumber;
            //Log.PrintLoadedDictionary(Players, "Log/Loading/PlayerTable_Debug.txt");
        }

    }
    private void CreateFiveBlank()
    {
        HeroDataBase.Instance.HeroDictionary.TryGetValue("Blank", out var blank); ;
        int totalNumber = 5;
        //创建人类玩家
        var newHumanPlayer = CreateHero(1, PlayerType.Human, blank);
        //加入玩家
        Players.Add(1, newHumanPlayer);
        //创建AI玩家
        for (int i = 2; i <= totalNumber; i++)
        {
            var newPlayer = CreateHero(i, PlayerType.AI, blank);
            //加入玩家
            Players.Add(i, newPlayer);
        }
        InitializePlayerSpace(totalNumber);
        AlivePlayerNumber = totalNumber;
    }
    private Player CreateHero(int ID_inGame, PlayerType playerType, HeroDefine heroDefine)
    {
        //创建玩家物体
        var newPlayerObject = Instantiate(playerPrefab,this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //初始化玩家脚本
        var newPlayer = newPlayerObject.GetComponent<Player>();
        newPlayer.Initialize(ID_inGame, playerType, heroDefine);
        InitializeUIText(newPlayer);
        InitializePlayerEffectController(newPlayer);
        return newPlayer;
    }
    #endregion
    #region Initialization Things
    private void InitializeUIText(Player newPlayer)
    {
        newPlayer.playerUIText.Initialize();
    }
    private void InitializePlayerEffectController(Player newPlayer)
    {
        newPlayer.playerEffectController.Initialize();
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
                //卡牌的选择和展示系统都需要知道哪位是玩家1，这里面第一个生成的人类玩家是玩家1，联机再说
                if (CardSelectionManager.Instance.player1 == null)
                {
                    CardSelectionManager.Instance.player1 = player;
                    CardDemonstrateSystem.Instance.AddListener(player);
                    CardPresentSystem.Instance.player1 = player;
                    RoundMonitor.Instance.player1 = player;
                }

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
