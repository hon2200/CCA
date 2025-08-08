using System.Collections;
using System.Collections.Generic;



public abstract class Skill
{
    public string Name;
    public string Discription;
}

//�ж�����Ч��������һ���ж�
public abstract class ActionSkill : Skill
{
    public string ActionID;
}

//����������ĳһ���׶Σ�����ʱ���Է���������ܣ�ͨ���ǻغϿ�ʼ�׶Σ��غϽ����׶λ򲹵��׶Ρ�
//�ж��׶κͽ���׶θ����������顣
public abstract class ActiveSkill : Skill
{
    public enum ActivePeriod
    {
        StartPhase = 1,
        EndPhase = 2,
        AdditionalPhase = 3
    }
    public ActivePeriod activePeriod;
}

//��������ĳ���ض�����´�����ͨ���ǽ���׶��м�
public abstract class TriggerSkill : Skill
{
    public abstract void OnAttackEffective();
    public abstract void OnDealWithDamage();
    public abstract void OnDefendActive();
    public abstract void OnCounterActive();
    public abstract void OnWounded();
    public abstract void OnGameStart();
    public abstract void OnNegativeHP();
}

//�ж�������ͨ��������ĳ���׶θı��ж��Ĳ���
public abstract class CookSkill : Skill
{
    public abstract void OnPreCook();
    public abstract void OnPostCook();
    public abstract void OnFirstCook();
}