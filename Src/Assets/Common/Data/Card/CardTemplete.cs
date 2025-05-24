using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

//通过ScriptableObject存储Card的所有需要序列化（i.e.在Inspector面板中显示）的组件
[CreateAssetMenu(fileName = "CardTemplete", menuName = "ScriptableObjects/CardTemplete", order = 1)]

public class CardTemplete : ScriptableObject
{
    //通过ID和ActionDataBase进行绑定
    //这个ID也只在创建卡牌的时候调用，其他时候不
    public string ID;
    //图像
    public Sprite image;
    //类别
    public ActionType actionType;
    //消耗图像，如果无消耗这个=null
    public Sprite comsume_image;
    //预制体//预制体只制造相应类别
    public GameObject prefab;
}
