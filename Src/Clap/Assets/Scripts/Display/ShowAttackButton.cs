using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAttackButton : MonoBehaviour
{
    //�����������choose target panel��confirm button�����棬�������������ж�������ʱ����
    //attackButton�ֶ���ֵ����ע��
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
