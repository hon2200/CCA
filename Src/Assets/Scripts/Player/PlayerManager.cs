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
        //Log.PrintLoadedDictionary(Players, "Log/Loading/PlayerTable_Debug.txt");
    }
    //����HP=5�����ڲ��Ե����
    private Player CreatePlayer_Debug(int ID_inGame,PlayerType playerType)
    {
        PlayerDefine playerDefine = new("Debug_Player", playerType, 5, new PlayerResource(0, 0, 0));
        PlayerStatus playerStatus = new(playerDefine);
        PlayerAction playerAction = new();
        //�����������
        var newPlayerObject = Instantiate(playerPrefab,this.transform);
        newPlayerObject.name = "Player" + ID_inGame;
        //��ʼ����ҽű�
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
        // ��ȡλ������
        //��Ҫά��������spacing��x��y���꣩�Ĳ����б���һ�£�δ�����ǸĽ�
        if (!PlayerSpacingDataBase.Instance.playerSpacingDictionary.TryGetValue(playerCount, out var spacingData))
        {
            Debug.LogError($"No spacing data found for player count: {playerCount}");
            return;
        }

        // ����λ���б����ƣ������޸�ԭʼ���ݣ�
        var availablePositions = new List<Vector2Int>();
        for (int i = 0; i < spacingData.Player_X.Count; i++)
        {
            availablePositions.Add(new Vector2Int(spacingData.Player_X[i], spacingData.Player_Y[i]));
        }

        // �ȴ���������ң�ȷ����������й̶�λ�ã�
        foreach (var player in Players.Values)
        {
            if (player.status.playerDefine.Type == PlayerType.Human)
            {
                // ������ҹ̶�λ��
                player.transform.localPosition = new Vector3(0, -300, 0);
            }
        }

        // Ȼ����AI���
        foreach (var player in Players.Values)
        {
            if (player.status.playerDefine.Type == PlayerType.AI && availablePositions.Count > 0)
            {
                // ʹ�ò��Ƴ���һ������λ��
                var pos = availablePositions[0];
                player.transform.localPosition = new Vector3(pos.x, pos.y, 0);
                availablePositions.RemoveAt(0);
            }
        }
    }
}
