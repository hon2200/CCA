using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;



//������дһ��ͨ��UI����Ӣ�۵ĺ���
public class PlayerManager : MonoSingleton<PlayerManager>
{
    public int AlivePlayerNumber;
    public GameObject playerPrefab;
    //ӵ����е�AI��Ԥ���壨Ҳ���ǹ��صĽű�����һ�£�
    public GameObject AIPrefab;
    public Dictionary<int, Player> Players;
    #region AI Things
    public void CreatingPlayers_BasedOnLevels()
    {
    }

    private Player CreateAI(int ID_inGame, AIDefine aIDefine)
    {
        //�����������
        var newPlayerObject = Instantiate(playerPrefab, this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //��ʼ����ҽű�
        var newPlayer = newPlayerObject.GetComponent<AIPlayer>();
        newPlayer.Initialize(ID_inGame, aIDefine);
        InitializeUIText(newPlayer);
        InitializePlayerEffectController(newPlayer);
        return newPlayer;
    }

    private Player CreateHuman_BasedOnLevel(int ID_inGame,int MaxHP)
    {
        //�����������
        var newPlayerObject = Instantiate(playerPrefab, this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //��ʼ����ҽű�
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
        //û�д�Initial Setting����
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
            //�����������
            var newHumanPlayer = CreateHero(1, PlayerType.Human, heroDefines[0]);
            //�������
            Players.Add(1, newHumanPlayer);
            //����AI���
            for (int i = 2; i <= totalNumber; i++)
            {
                var newPlayer = CreateHero(i, PlayerType.AI, heroDefines[i - 1]);
                //�������
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
        //�����������
        var newHumanPlayer = CreateHero(1, PlayerType.Human, blank);
        //�������
        Players.Add(1, newHumanPlayer);
        //����AI���
        for (int i = 2; i <= totalNumber; i++)
        {
            var newPlayer = CreateHero(i, PlayerType.AI, blank);
            //�������
            Players.Add(i, newPlayer);
        }
        InitializePlayerSpace(totalNumber);
        AlivePlayerNumber = totalNumber;
    }
    private Player CreateHero(int ID_inGame, PlayerType playerType, HeroDefine heroDefine)
    {
        //�����������
        var newPlayerObject = Instantiate(playerPrefab,this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //��ʼ����ҽű�
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
        // ��ȡλ������
        //��Ҫά��������spacing��x��y���꣩�Ĳ����б���һ�£�δ�����ǸĽ�
        if (!PlayerSpacingDataBase.Instance.playerSpacingDictionary.TryGetValue(playerCount, out var spacingData))
        {
            Debug.LogError($"No spacing data found for player count: {playerCount}");
            return;
        }

        // ����λ���б����ƣ������޸�ԭʼ���ݣ�
        var availablePositions = new List<Vector2>();
        for (int i = 0; i < spacingData.Player_X.Count; i++)
        {
            availablePositions.Add(new Vector2(spacingData.Player_X[i], spacingData.Player_Y[i]));
        }

        // �ȴ���������ң�ȷ����������й̶�λ�ã�
        foreach (var player in Players.Values)
        {
            if (player.playerType == PlayerType.Human)
            {
                // ������ҹ̶�λ��
                player.transform.localPosition = new Vector3(8, -5, 1);
                //���Ƶ�ѡ���չʾϵͳ����Ҫ֪����λ�����1���������һ�����ɵ�������������1��������˵
                if (CardSelectionManager.Instance.player1 == null)
                {
                    CardSelectionManager.Instance.player1 = player;
                    CardDemonstrateSystem.Instance.AddListener(player);
                    CardPresentSystem.Instance.player1 = player;
                    RoundMonitor.Instance.player1 = player;
                }

            }
        }

        // Ȼ����AI���
        foreach (var player in Players.Values)
        {
            if (player.playerType == PlayerType.AI && availablePositions.Count > 0)
            {
                // ʹ�ò��Ƴ���һ������λ��
                var pos = availablePositions[0];
                player.transform.localPosition = new Vector3(pos.x, pos.y, 1);
                availablePositions.RemoveAt(0);
            }
        }
    }
    #endregion
}
