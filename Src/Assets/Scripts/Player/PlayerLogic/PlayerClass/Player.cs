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

    //创建一个英雄类型的玩家
    public void Initialize(int ID_inGame, PlayerType playerType, HeroDefine heroDefine)
    {
        this.ID_inGame = ID_inGame;
        this.status = new(heroDefine.MaxHP, new());
        this.action = new();
        this.playerType = playerType;
        //添加英雄技能
        foreach(var hero in HeroDataBase.Instance.HeroDictionary)
        {
            if (hero.Key == heroDefine.ID)
                foreach(var skillID in hero.Value.SkillIDList)
                {
                    SkillLiberary.Instance.skillDic.TryGetValue(skillID, out var skill);
                    this.hero.skills.Add(skill);
                }
        }
        isReady = new ReadyAttribute();
        isReady.Cancel();
    }
}