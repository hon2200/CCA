using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public int ID_inGame { get; set; }
    public string Name { get; set; }
    public PlayerStatus status { get; set; }
    public PlayerAction action { get; set; }
    public PlayerType playerType { get; set; }
    public Hero hero { get; set; }
    public ReadyAttribute isReady { get; set; }

    public PlayerUIText playerUIText;
    public PlayerEffectController playerEffectController;
    //Available Actions?????????????????��???????????????��????��?+???????????��?
    public List<string> AvailableActions { get; set; }
    //????��???????????AvailableActions??????????????????????????��??��?
    public List<string> ForbiddenActions { get; set; }
    public List<int> possibleKillers { get; set; }
    public Action OnBirth { get; set; }
    public bool is_stun = false;
    public CDManager CDmanager { get; set; }

    protected void Initialize(int ID_inGame, string Name, PlayerType playerType,
        int MaxHP, List<int> InitialResource = null, List<string> AvailableActions = null,
         string heroID = "Blank" , List<string> skills = null)
    {
        this.ID_inGame = ID_inGame;
        this.Name = Name;
        if (InitialResource == null)
            InitialResource = new() { 0, 0, 0 };
        this.status = new(MaxHP, InitialResource);
        this.status.resources.Bullet.Owner = this;
        this.status.resources.Sword.Owner = this;
        this.action = new();
        this.playerType = playerType;
        this.status.buffs.BuffOwner = this;
        hero = new(this, heroID, skills);
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
        ForbiddenActions = new();
        foreach(var skill in hero.skills)
        {
            if (skill is ActionSkill actionSkill)
                actionSkill.AddingAvailableAction(this);
        }
        OnBirth += () =>
        {
            playerUIText.Initialize();
            playerEffectController.Initialize();
            status.life.Born();
        };

        status.HP.OnValueChanged += (oldHP,newHP,meassage) =>
        {

        };

        possibleKillers = new();

        CDmanager = new();
    }

    public void SetHero(Hero hero)
    {
        if (hero == null)
            return;
        this.hero = hero;
        hero.SetSkillOwners(this);
        status.MaxHP = hero.MaxHP;
        status.HP.Set(hero.CurrentHP.Value);
        playerUIText?.UpdateSkillText();
    }

    public void SwitchHero(Hero hero)
    {
        if (hero == null)
            return;
        if (ReferenceEquals(this.hero, hero))
            return;
        Hero oldHero = this.hero;
        if (oldHero != null)
        {
            oldHero.CurrentHP.Set(status.HP.Value);
        }
        SetHero(hero);
    }
}