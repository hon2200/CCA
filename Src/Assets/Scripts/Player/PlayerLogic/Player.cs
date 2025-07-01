using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//创建Player需要注意的问题：
//如果我对Player类进行内部结构调整，只需要改一改Initialize就好。
public class Player : MonoBehaviour
{
    public int ID_inGame { get; set; }
    public PlayerStatus status { get; set; }
    public PlayerAction action { get; set; }
    public PlayerUIText playerUIText;
    public ReadyAttribute isReady;
    public void Initialize(int ID_inGame, PlayerStatus status, PlayerAction action)
    {
        this.ID_inGame = ID_inGame;
        this.status = status;
        this.action = action;
        isReady = new ReadyAttribute();
        isReady.Cancel();
    }
    //玩家完成对某一个行动资源的消耗
    public void Comsume(ActionBase actionBase)
    {
        status.resources.Bullet.Use(actionBase.ActionInfo.Costs[0]);
        status.resources.AvailableSword.Use(actionBase.ActionInfo.Costs[1]);
    }

}