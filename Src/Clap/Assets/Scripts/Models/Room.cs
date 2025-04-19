using Common.Data;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
public struct RoundMove
{
    public int id;
    public ItemType type;
    public string Name;
    public int cardid;
    public List<int> targetid;
}

public class RoundChanged
{
    public int id;
    public float HpChanged;
    public int BulletChanged;
    public int SwardChanged;
    public int InCDChanged;
    public float MaxHpChanged;
    public Dictionary<int, BuffDefine> buffs;
}
public class Room : Singleton<Room>
{
    public int id = 1;
    public RoomDefine room;

    public Dictionary<int, List<RoundMove>> RoundMoves = new Dictionary<int, List<RoundMove>>();
    public Dictionary<int, List<RoundChanged>> RoundChangeds = new Dictionary<int, List<RoundChanged>>();


    public void NewRoom()
    {
        id = 1;
        room = new RoomDefine();
        Battle.Instance.OnEnable();
    }

    internal void Add(int iD, CharacterSet characterSet)
    {
        room.NumberID.Add(iD);
        Battle.Instance.Add(iD);
        BattleManager.Instance.Characters.TryGetValue(id,out CharacterData data);
        data.Init(int.Parse(characterSet.HP.text), iD);
        BattleManager.Instance.Characters[id++] = data;
    }

    public int GetLastEnemyAction(int enemyId)
    {
        RoundMoves.TryGetValue(Battle.Instance.currentRound - 1, out var enemys);
        foreach (var enemy in enemys)
        {
            if (enemy.id == enemyId)
                return enemy.cardid;
        }
        return 0;
    }


}
