using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerisReady : MonoBehaviour
{
    //Player List��Ҫ�ֶ���ֵ ��Ҫע��.instant����Ҫ
    public List<GameObject> Player;
    public List<Instant> instant;
    public void Awake()
    {
        for(int i = 0; i < Player.Count; i++)
        {
            instant.Add(Player[i].GetComponent<Instant>());
        }
    }
    public void ReadinInstants()
    {
        for(int i = 0; i < instant.Count; i++)
            instant[i].PerformAct();
    }
}
