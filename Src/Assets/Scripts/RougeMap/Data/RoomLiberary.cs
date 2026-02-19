using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using UnityEngine;

//房间图书馆，通过和ActionDataBase一样的Key键索引卡牌，通过CardTemplete获得预制体卡牌以及其实例所需要的资源
public class RoomLiberary : MonoSingleton<RoomLiberary>
{
    //所有房间汇总
    public SerializedDictionary<RoomID, RoomTemplete> RoomDictionary;
}