using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public int ID_inGame { get; set; }
    public PlayerStatus status { get; set; }
    public PlayerAction action { get; set; }
    public PlayerType playerType { get; set; }
    public Hero hero { get; set; }
    public ReadyAttribute isReady { get; set; }

    public PlayerUIText playerUIText;
    public PlayerEffectController playerEffectController;
    public List<string> AvailableActions;
    public Action OnBirth;
    public bool is_stun = false;

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
        if (heroDefine != null)
            this.hero = new(this, heroDefine);
        else
        {
            HeroDataBase.Instance.HeroDictionary.TryGetValue("Blank", out var blank);
            this.hero = new(this, blank);
        }
        isReady = new ReadyAttribute();
        isReady.Cancel();
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

        status.HP.OnValueChanged += (oldHP,newHP,meassage) =>
        {

        };

    }
}