using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

//通过ScriptableObject存储Card的所有需要序列化（i.e.在Inspector面板中显示）的组件
[CreateAssetMenu(fileName = "RoomTemplete", menuName = "ScriptableObjects/RoomTemplete", order = 1)]

public class RoomTemplete : ScriptableObject
{
    //通过ID和ActionDataBase进行绑定
    //这个ID也只在创建卡牌的时候调用，其他时候不
    public RoomID ID;
    //图像
    public Sprite image;
}


public enum RoomID
{
    StartRoom = -1,
    Undecided = 0,

    Boss = 1,
    Elite = 2,
    Minion = 3,

    TalentMarket = 10,
    AntiqueMarket = 11,
    CardMarket = 12,
    Alchemyworkshop = 13,
    TailorShop = 14,
    CardReward = 15,

    Tavern = 17,
    SacredCemetery = 18,
    SoulFountain = 19,

    DemonAlter = 30,

    EvilForge = 40,
    CurseFusion = 41,
    Treasure = 42
}