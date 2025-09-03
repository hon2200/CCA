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

    //����һ��Ӣ�����͵����
    public void Initialize(int ID_inGame, PlayerType playerType, HeroDefine heroDefine)
    {
        this.ID_inGame = ID_inGame;
        this.status = new(heroDefine.MaxHP, new());
        this.action = new();
        this.playerType = playerType;
        //���Ӣ�ۼ���
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