using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAttackButton : MonoBehaviour
{
    //这个东西挂在choose target panel的confirm button的下面，当攻击或挑衅行动被按下时触发
    //attackButton手动赋值，需注意
    public List<GameObject> attackButton;
    public void Update()
    {
        if(gameObject.activeSelf)
            for(int i = 0; i < attackButton.Count; i++)
                attackButton[i].SetActive(true);
    }
    public void TurnOffAttackButton()
    {
        for (int i = 0; i < attackButton.Count; i++)
            attackButton[i].SetActive(false);
    }
}
