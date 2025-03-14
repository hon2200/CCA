using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSwitch : MonoBehaviour
{
    //这里使用find必不可少，因为是从子菜单往上能找到Action Panel，要将Action Panel下面的行动主面板激活；
    //包括后面的choose your target也是一样的。
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