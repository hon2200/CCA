using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//创建Player需要注意的问题：
//如果我对Player类进行内部结构调整，只需要改一改Initialize就好。
//但是我在Player预制体里面拖拽实现调用，这要求我不能再VS或其他Unity外部去调整这个文件的位置
//我很讨厌Player和PlayerUIText，PlayerEffectController的双向引用，之后想办法简化一下
public class Player : MonoBehaviour
{
    public int ID_inGame { get; set; }
    //玩家状态
    public PlayerStatus status { get; set; }
    //玩家行动
    public PlayerAction action { get; set; }
    public PlayerType playerType { get; set; }
    public Hero hero { get; set; }
    public ReadyAttribute isReady { get; set; }

    public PlayerUIText playerUIText;
    public PlayerEffectController playerEffectController;
    //可用行动列
    public List<string> AvailableActions;
    public Action OnBirth; 

    //创建玩家
    protected void Initialize(int ID_inGame, PlayerType playerType,
        int MaxHP, List<int> InitialResource = null, List<string> AvailableActions = null,
         HeroDefine heroDefine = null)
    {
        this.ID_inGame = ID_inGame;
        if (InitialResource == null)
            InitialResource = new() { 0, 0, 0 };
        this.status = new(MaxHP, InitialResource);
        this.action = new();
        this.playerType = playerType;
        //处理英雄：如果没有赋值，则为其赋值白板
        if (heroDefine != null)
            this.hero = new(this, heroDefine);
        else
        {
            HeroDataBase.Instance.HeroDictionary.TryGetValue("Blank", out var blank);
            this.hero = new(this, blank);
        }
        isReady = new ReadyAttribute();
        isReady.Cancel();
        //处理可用行动：如果没有为其赋值，则为其赋值基础行动
        if (AvailableActions != null)
        {
            this.AvailableActions = new();
            foreach (var action in AvailableActions)
            {
                this.AvailableActions.Add(action);
            }
        }
        else
        {
            this.AvailableActions = new();
            foreach (var action in ActionDataBase.Instance.ActionDictionary.Values)
            {
                if (action.isBasic)
                    this.AvailableActions.Add(action.ID);
            }
        }
        //英雄可能带有的行动技
        foreach(var skill in hero.skills)
        {
            if (skill is ActionSkill actionSkill)
                actionSkill.AddingAvailableAction(this);
        }
        OnBirth += () =>
        {
            playerUIText.Initialize();
            playerEffectController.Initialize();
        };

        //玩家受伤时，调用Ondamaged
        status.HP.OnValueChanged += (oldHP,newHP,meassage) =>
        {

        };

    }
}