using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundMonitor : MonoSingleton<RoundMonitor>
{
    public Player player1;
    public Button button;
    public void Update()
    {
        if (player1 == null)
        {
            button.interactable = false;
        }
        if(player1 != null)
        {
            if (player1.action.Count == 0)
                button.interactable = false;
            else
                button.interactable = true;
        }
    }
}
