using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

//����洢�˸���˲ʱֵ�����𵽵�������ڲ���move��instant�Ĵ���
//��ԭ���ٵĹ��ܣ�damagesource����ͷ������㣨��Ҫ�ڴ�ת���Ͻ��У�
public class Instant : MonoBehaviour
{
    //��Ҫ֪����������˭
    public PlayerNumber playerNumber;
    //��Ҫ֪����Ϸ�ж�����,�ֶ���ֵ
    public People people;
    //��Ҫ֪����ҽ�����ʲô�ж�
    public Move move;
    //��Ҫ֪��Status�������ı�����ֻ������status�е�consume
    public Status status;
    //�߸�˲ʱֵ
    public List<Charge> charge = new();//charge[�ж����],��ֵ��Add
    public List<int> block = new(), ca = new();//block&ca[�ж����]����ֵ��������򷴵�������
    public List<bool> doblock = new(), doca = new();//doblock&daca[��������]//[��������]//��ֵ�����Ƿ�����˸÷����򷴵�
    public List<Attack> attack = new();//attack[�ж����]��ֵ��Attack 
    public List<int> provoke = new();//provoke[�ж����]����ֵ�������ƶ���
    public bool comeon = new();//comeon����ֵ�����Ƿ�����˹���
                               //��������˲ʱֵ(ע������)
    public bool ready = false;
    public void PerformAct()
    {
        for (int order = 0; order < move.move.Count; order++)
        {
            status.Consume(order);
            switch (move.catagory[order])
            {
                case 1:
                    ActCharge(order);
                    break;
                case 2:
                    ActShoot(order);
                    break;
                case 3:
                    ActStab(order);
                    break;
                case 4:
                    ActBlock(order);
                    break;
                case 5:
                    ActCounterattack(order);
                    break;
                case 6:
                    ActProvoke(order);
                    ActComeon(order);
                    break;
            }
        }
        ready = true;
    }//��move�еı������ݵ�����˲ʱʱֵ�У�
    //����Player�ĵ�order���ж�������move����ֵshoot��attack��ds
    //ȱ���Ľ��㣺��ƽ���Լ����˺�and�����ڵ��˺���
    public void ActShoot(int order)
    {
        int to = move.at[order];
        switch (move.move[order])
        {
            case 1://shoot
                attack.Add(new Attack(1, 4, to, 1, 0));
                break;
            case 2://double shoot
                attack.Add(new Attack(1, 4, to, 2, 0));
                break;
            case 3://lasergun
                attack.Add(new Attack(2, 4, to, 3, 0));
                break;
            case 4://lasercannon:damage��perform attack�����ٸ�ֵ
                attack.Add(new Attack(0, 1, to, 4, 0));
                break;
            case 5://RPG
                attack.Add(new Attack(2, 6, to, 5, 0));
                break;
            case 6://double RPG
                attack.Add(new Attack(3, 6, to, 6, 0));
                break;
            case 7://Peace
                attack.Add(new Attack(3, 7, to, 7, 0));
                for (int i = 1; i <= people.initialPeople; i++)//��ƽ��ͻ����λ 
                {
                    attack.Add(new Attack(1, 7, to, 7, 0));
                }
                break;
            default:
                Debug.Log("shoot has other values?");
                break;
        }
    }
    //����move����ֵblock��doblock
    public void ActBlock(int order)
    {
        //ע��doblock�ĳ��ȴ���û�б仯��
        //doblock[0]��true����û�н����κη����ж�������д�������һЩ����
        switch (move.move[order])
        {
            case 1: block.Add(1); doblock[1] = true; break;
            case 2: block.Add(2); doblock[2] = true; break;
            case 3: block.Add(3); doblock[3] = true; break;
            case 4: block.Add(4); doblock[4] = true; break;
        }
        //doblock[0]��Ϊ�棬�����û�н����κη����ж�������д�ܴ���һЩ����
        if (doblock[1] == false && doblock[2] == false && doblock[3] == false && doblock[4] == false)
            doblock[0] = true;
    }
    //����move����ֵstab��attack
    //ds������û�б���ֵ
    public void ActStab(int order)
    {
        int to = move.at[order];
        switch (move.move[order])
        {
            case 1://stab
                attack.Add(new Attack(1, 2, to, 0, 1));
                break;
            case 2://double stab
                attack.Add(new Attack(1, 3, to, 0, 2));
                break;
            case 3://lightsaber stab
                attack.Add(new Attack(2, 2, to, 0, 3));
                break;
            case 4://lightsaber chop
                attack.Add(new Attack(2, 3, to, 0, 4));
                break;
            case 5://double chop
                attack.Add(new Attack(1, 3, to, 0, 5));
                break;
            case 6://ghost chop
                attack.Add(new Attack(1, 5, to, 0, 6));
                break;
            case 7://nock-end chop
                attack.Add(new Attack(2, 5, to, 0, 7));
                break;
            default:
                Debug.Log("Stab has other values?");
                break;
        }
    }
    //����move����ֵadd
    public void ActCharge(int order)
    {
        int to = move.at[order];
        switch (move.move[order])
        {
            case 1:
                charge.Add(new Charge(1, 1, playerNumber.number));
                break;
            case 2:
                charge.Add(new Charge(2, 1, playerNumber.number));
                break;
            default:
                Debug.Log("Add has other values?");
                break;
        }
    }
    //����move����ֵca
    public void ActCounterattack(int order)
    {
        switch (move.move[order])
        {
            case 1: ca.Add(1); doca[1] = true; break;
            case 2: ca.Add(2); doca[2] = true; break;
            case 3: ca.Add(3); doca[3] = true; break;
            case 4: ca.Add(4); doca[4] = true; break;
            default:
                Debug.Log("ca has other values?");
                break;
        }
        //doca[0]��true����û�н����κη����ж�������д�������һЩ����
        if (doca[1] == false && doca[2] == false && doca[3] == false && doca[4] == false)
            doca[0] = true;
    }
    //����move����ֵprovoke
    public void ActProvoke(int order)
    {
        int to = move.at[order];
        if (move.move[order] == 1)
            provoke.Add(to);
    }
    //����move����ֵcomeon
    public void ActComeon(int order)
    {
        if (move.move[order] == 2)
            comeon = true;
    }
    public void ClearInstants()
    {
        charge.Clear();
        block.Clear();
        ca.Clear();
        for (int i = 0; i < doblock.Count; i++) 
        {
            doblock[i] = false;
        }
        for(int i=0;i<doca.Count;i++)
        {
            doca[i] = false;
        }
        attack.Clear();
        provoke.Clear();
        comeon = false;
    }
}
//����˲ʱֵ�Ľṹ��
[System.Serializable]//ʹ�����class�ܹ���inspector��忴��
public class Charge
{
    public int catagory;//���ͣ�1=�ӵ���2=��
    public int number;//һ�β���������
    public int target;
    public Charge(int catagory,int number,int target)
    {
        this.catagory = catagory;
        this.number = number;
        this.target = target;
    }
};
//attack˲ʱֵ�Ľṹ��
//�洢�����йصĲ���
//�洢���������˺�
[System.Serializable]//ʹ�����class�ܹ���inspector��忴��
public class Attack
{
    public int damage;//�˺�
    public int level;//�ȼ�//������һ��Ĭ�ϣ���attack level==0�����û�н����κι����ж��ĳ�Ҫ�������������Ƶ��ж��У�
    public int target;//����Ŀ��
    public int shoot;//ʹ�����������͵��ӵ�����
    public int stab;//ʹ�����������͵ĵ�������
    public Attack(int damage, int level, int target, int shoot, int stab)
    {
        this.damage = damage;
        this.level = level;
        this.target = target;
        this.shoot = shoot;
        this.stab = stab;   
    }
};



