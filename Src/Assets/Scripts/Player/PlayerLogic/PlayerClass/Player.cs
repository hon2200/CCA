using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//����Player��Ҫע������⣺
//����Ҷ�Player������ڲ��ṹ������ֻ��Ҫ��һ��Initialize�ͺá�
//��������PlayerԤ����������קʵ�ֵ��ã���Ҫ���Ҳ�����VS������Unity�ⲿȥ��������ļ���λ��
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
}