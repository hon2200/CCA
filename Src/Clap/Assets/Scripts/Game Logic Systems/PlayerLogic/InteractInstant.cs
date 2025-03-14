using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这个类描述了player之间互动的变量，这些没办法直接赋值，只能在wheelLogic里面赋值
[System.Serializable]
public class InteractInstant : MonoBehaviour
{
    public People people;//需要知道有多少人
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
