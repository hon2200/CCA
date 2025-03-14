using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSwitch : MonoBehaviour
{
    //����ʹ��find�ز����٣���Ϊ�Ǵ��Ӳ˵��������ҵ�Action Panel��Ҫ��Action Panel������ж�����弤�
    //���������choose your targetҲ��һ���ġ�
    public void TurnOffParent()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }
    public void TurnOffSelfAndReturnToParent()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
        gameObject.transform.parent.parent.Find("Action Main Panel").gameObject.SetActive(true);
    }
    public void TurnOffAndSelectTarget()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
        gameObject.transform.parent.parent.Find("Choose Targets Panel").gameObject.SetActive(true);
    }
}