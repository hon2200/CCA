using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using UnityEngine;

//卡牌图书馆，通过和ActionDataBase一样的Key键索引卡牌，通过CardTemplete获得预制体卡牌以及其实例所需要的资源
public class CardLiberary : MonoSingleton<CardLiberary>
{
    [SerializeField]
    public SerializedDictionary<string, CardTemplete> CardDictionary;
}
