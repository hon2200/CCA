using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//����Player��Ҫע������⣺
//����Ҷ�Player������ڲ��ṹ������ֻ��Ҫ��һ��Initialize�ͺá�
//��������PlayerԤ����������קʵ�ֵ��ã���Ҫ���Ҳ�����VS������Unity�ⲿȥ��������ļ���λ��
//�Һ�����Player��PlayerUIText��PlayerEffectController��˫�����ã�֮����취��һ��
public class Player : MonoBehaviour
{
    public int ID_inGame { get; set; }
    //���״̬
    public PlayerStatus status { get; set; }
    //����ж�
    public PlayerAction action { get; set; }
    public PlayerType playerType { get; set; }
    public Hero hero { get; set; }
    public ReadyAttribute isReady { get; set; }

    public PlayerUIText playerUIText;
    public PlayerEffectController playerEffectController;
    //�����ж���
    public List<string> AvailableActions;
    public Action OnBirth;
    public bool is_stun = false;

    //�������
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
        //����Ӣ�ۣ����û�и�ֵ����Ϊ�丳ֵ�װ�
        if (heroDefine != null)
            this.hero = new(this, heroDefine);
        else
        {
            HeroDataBase.Instance.HeroDictionary.TryGetValue("Blank", out var blank);
            this.hero = new(this, blank);
        }
        isReady = new ReadyAttribute();
        isReady.Cancel();
        //��������ж������û��Ϊ�丳ֵ����Ϊ�丳ֵ�����ж�
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
        //Ӣ�ۿ��ܴ��е��ж���
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

        //�������ʱ������Ondamaged
        status.HP.OnValueChanged += (oldHP,newHP,meassage) =>
        {

        };

    }
}