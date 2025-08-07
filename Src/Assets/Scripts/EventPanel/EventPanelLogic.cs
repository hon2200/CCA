using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventPanelLogic : MonoSingleton<EventPanelLogic>
{
    public GameObject Panel;
    public void CloseEventPanel()
    {
        Panel.SetActive(false);
    }
    public void OpenEventPanel()
    {
        Panel.SetActive(true);
    }
    public void CloseorOpenEventPanel()
    {
        if (Panel.activeSelf)
            Panel.SetActive(false);
        else
            Panel.SetActive(true);
    }
}
