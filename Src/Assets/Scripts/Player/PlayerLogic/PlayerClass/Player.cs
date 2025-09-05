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
        this.status = new(heroDefine.MaxHP, new (){ 0, 0, 0 });
        this.action = new();
        this.playerType = playerType;
        this.hero = new(heroDefine);
        isReady = new ReadyAttribute();
        isReady.Cancel();
    }
}