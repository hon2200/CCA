using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��û��д��ƽ
public class WheelLogic : MonoBehaviour
{
    //��Ҫ֪��һ�������ˣ��ֶ���ֵ
    public People People;
    //��Ҫ֪��ÿ����ң��ֶ���ֵ
    //ע���±�Ϊ��ұ�ţ����Գ���ΪPlayer������+1
    public List<GameObject> Player;
    //��Ҫ֪��ÿ����ҵ�״̬��˲ʱֵ����ɽ�����������Awake�����
    //��Ҫ֪����ҵ�move���Ա����µ�һ�غ�����֮ǰ����
    public List<Move> move;
    public List<Status> status;
    public List<Instant> instant;
    public List<InteractInstant> interactInstant;
    
    public void Awake()
    {
        //��ʼ��Status��Instant��InteractInstant
        //��������Ϊ��������ĵ�һ��Ԫ�ز����븳ֵ
        move.Add(null);
        instant.Add(null);
        status.Add(null);
        interactInstant.Add(null);
        for (int people = 1; people <= People.initialPeople; people++)
        {
            move.Add(Player[people].GetComponent<Move>());
            instant.Add(Player[people].GetComponent<Instant>());
            status.Add(Player[people].GetComponent<Status>());
            interactInstant.Add(Player[people].GetComponent<InteractInstant>());
        }
    }
    //������ҿ�����һ���ǳ���Ч������//������λ����Ƿ�׼������
    public void Update()
    {
        int readyNumber = 0;
        for (int people = 1; people <= People.initialPeople; people++)
        {
            if (instant[people].ready)
                readyNumber++;
        }
        if(readyNumber == People.alivePeople)
        {
            Spinning();
            //����תһ��֮������������½������״̬��
            for(int people= 1; people <= People.initialPeople; people++)
            {
                instant[people].ready = false;
                instant[people].ClearInstants();
                move[people].ClearMoves();
            }
        }
    }
    public void Spinning()
    {
        PerformComeon();
        PerformCharge();
        CalculateMaxAttackLevel();
        PerformAttack();
    }
    //�Ƚ��й����Ĺ���Ŀ��ı�
    public void PerformComeon()
    {
        List<int> comeonPeople = new();
        for (int people = 1; people <= People.initialPeople; people++) 
        {
            if (instant[people].comeon==true)
            {
                comeonPeople.Add(people);
            }
        }
        if (comeonPeople.Count > 0)  
        {
            for(int people = 1; people <= People.initialPeople; people++)
            {
                //�Ƚ����йع�����Ŀ��ı�
                int howManyAttack = instant[people].attack.Count;
                for (int order = 0; order < howManyAttack; order++)
                {
                    for (int i = 0; i < comeonPeople.Count; i++)
                    {
                        //�����������ж�����
                        instant[people].attack.Add(instant[people].attack[order]);
                        //����������Ŀ��ı䣬�Ѹռ�����ж�Ŀ��ĳɹ������Ǹ���
                        instant[people].attack[instant[people].attack.Count - 1].target = comeonPeople[i];
                    }
                }
                //��ԭ�����ж�ɾ��
                for (int order = 0; order < howManyAttack; order++) 
                {
                    instant[people].attack.RemoveAt(order);
                }
                //����ͬ���ķ���
                int howManyCharge = instant[people].charge.Count;
                for(int order = 0; order < howManyCharge; order++)
                {
                    for (int i = 0; i < comeonPeople.Count; i++)
                    {
                        //�����������ж�����
                        instant[people].charge.Add(instant[people].charge[order]);
                        //����������Ŀ��ı䣬�Ѹռ�����ж�Ŀ��ĳɹ������Ǹ���
                        instant[people].charge[instant[people].charge.Count - 1].target = comeonPeople[i];
                    }
                }
                //��ԭ�����ж�ɾ��
                for (int order = 0; order < howManyAttack; order++)
                {
                    instant[people].charge.RemoveAt(order);
                }
            }
        }
    }
    //��ҿ�ʼ���в����
    public void PerformCharge()
    {
        for (int people = 1; people <= People.initialPeople; people++)
        {
            for (int order = 0; order < instant[people].charge.Count; order++)
            {
                //����Ŀ�����Դ��+=�����ж��߲��������������ӵ��ͽ�
                if (instant[people].charge[order].catagory == 1)
                    status[instant[people].charge[order].target].bullet += instant[people].charge[order].number;
                if (instant[people].charge[order].catagory == 2)
                    status[instant[people].charge[order].target].sword += instant[people].charge[order].number;
            }
        }
    }
    //��ֵmaxAttackLevel
    public void CalculateMaxAttackLevel()
    {
        for (int people = 1; people <= People.initialPeople; people++)
        {
            for (int order = 0; order < instant[people].attack.Count; order++)
            {
                //����¹����Ĺ�������ԭ������
                if (instant[people].attack[order].level > interactInstant[people].maxAttackLevel[instant[people].attack[order].target])
                {
                    interactInstant[people].maxAttackLevel[instant[people].attack[order].target] = instant[people].attack[order].level;
                }
            }
        }
    }
    //��ʼ������
    public void PerformAttack()
    {
        for(int from= 1; from <= People.initialPeople; from++)
        {
            for (int order = 0; order < instant[from].attack.Count; order++)
            {
                int to = instant[from].attack[order].target;
                //�������ж�
                if (interactInstant[from].maxAttackLevel[to] > interactInstant[to].maxAttackLevel[from])
                {
                    switch (instant[from].attack[order].shoot)
                    {
                        case 1:
                            if (instant[to].doblock[1] == false && instant[to].doca[1] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[1] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].damageSource[from] = true;
                            }
                            break;
                        case 2:
                            if (instant[to].doblock[2] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            break;
                        case 3:
                            if (instant[to].doblock[1] == false && instant[to].doca[1] == false && instant[to].doca[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[1] == true || instant[to].doca[4] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].possibleKiller[from] = true;
                            }
                            break;
                        case 4:
                            if (instant[to].doblock[0] == true && instant[to].doca[1] == false && instant[to].doca[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[1] == true || instant[to].doca[4] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].possibleKiller[from] = true;
                            }
                            break;
                        case 5:
                            if (instant[to].doblock[1] == false && instant[to].doca[1] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[1] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].possibleKiller[from] = true;
                            }
                            break;
                        case 6:
                            if (instant[to].doblock[2] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            break;
                            //default:
                            //;//essencial???
                    }
                    switch (instant[from].attack[order].stab)
                    {
                        case 1:
                            if (instant[to].doblock[1] == false && instant[to].doblock[2] == false && instant[to].doca[1] == false
                                && instant[to].doca[2] == false && instant[to].doca[3] == false && instant[to].doca[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[2] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].possibleKiller[from] = true;
                            }
                            if (instant[to].doca[3] == true)
                            {
                                status[from].swordinCD = status[from].sword;
                            }
                            break;
                        case 2:
                            if (instant[to].doblock[3] == false && instant[to].doca[3] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[3] == true)
                            {
                                status[from].swordinCD = status[from].sword;
                            }
                            break;
                        case 3:
                            if (instant[to].doblock[1] == false && instant[to].doblock[2] == false && instant[to].doca[1] == false
                                && instant[to].doca[2] == false && instant[to].doca[3] == false && instant[to].doca[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[2] == true || instant[to].doca[4] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].possibleKiller[from] = true;
                            }
                            if (instant[to].doca[3] == true)
                            {
                                status[from].swordinCD = status[from].sword;
                            }
                            break;
                        case 4:
                            if (instant[to].doblock[3] == false && instant[to].doca[3] == false && instant[to].doca[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            if (instant[to].doca[4] == true)
                            {
                                status[from].HP = status[from].HP - instant[from].attack[order].damage;
                                interactInstant[to].possibleKiller[from] = true;
                            }
                            if (instant[to].doca[3] == true)
                            {
                                status[from].swordinCD = status[from].sword;
                            }
                            break;
                        case 5:
                            if (instant[to].doblock[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            break;
                        case 6:
                            if (instant[to].doblock[4] == false)
                            {
                                status[to].HP = status[to].HP - instant[from].attack[order].damage;
                                interactInstant[from].damageSource[to] = true;
                                interactInstant[from].possibleKiller[to] = true;
                            }
                            break;
                    }
                }
            }
        }
    }
    
}