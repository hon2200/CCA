using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����������player֮�以���ı�������Щû�취ֱ�Ӹ�ֵ��ֻ����wheelLogic���渳ֵ
[System.Serializable]
public class InteractInstant : MonoBehaviour
{
    public People people;//��Ҫ֪���ж�����
    public List<bool> possibleKiller;
    public List<bool> damageSource;
    public List<int> maxAttackLevel;
    public void Awake()
    {
        for(int i = 0; i <= people.initialPeople; i++)
        {
            possibleKiller.Add(false);
            damageSource.Add(false);
            maxAttackLevel.Add(0);
        }
    }
}
