using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMain : MonoBehaviour
{
    public void OnSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }
}
