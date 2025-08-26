using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    public int AlivePlayerNumber;
    public GameObject playerPrefab;
    public Dictionary<int, Player> Players;
    public void CreatingPlayers_Debug()
    {
        Players = new Dictionary<int, Player>();
        //�����������
        var newHumanPlayer = CreatePlayer_Debug(1, PlayerType.Human);
        //�������
        Players.Add(1, newHumanPlayer);
        //����AI���
        for (int i = 2; i <= 5; i++)
        {
            var newPlayer = CreatePlayer_Debug(i, PlayerType.AI);
            //�������
            Players.Add(i, newPlayer);
        }
        InitializePlayerSpace(5);
        AlivePlayerNumber = 5;
        //Log.PrintLoadedDictionary(Players, "Log/Loading/PlayerTable_Debug.txt");
    }
    //����HP=5�����ڲ��Ե����
    private Player CreatePlayer_Debug(int ID_inGame,PlayerType playerType)
    {
        HeroDefine playerDefine = new("Debug_Player", 5, new PlayerResource(0, 0, 0));
        PlayerStatus playerStatus = new(playerDefine);
        PlayerAction playerAction = new();
        //�����������
        var newPlayerObject = Instantiate(playerPrefab,this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //��ʼ����ҽű�
        var newPlayer = newPlayerObject.GetComponent<Player>();
        newPlayer.Initialize(ID_inGame, playerStatus, playerAction, playerType);
        InitializeUIText(newPlayer);
        InitializePlayerEffectController(newPlayer);
        return newPlayer;
    }
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
}
