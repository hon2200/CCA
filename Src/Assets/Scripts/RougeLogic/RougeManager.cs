using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RougeManager : MonoSingleton<RougeManager>
{
    public RougePlayer rougePlayer;

    private void Awake()
    {
        rougePlayer = new RougePlayer();
        rougePlayer.Relics.OnListChanged = (list, message) =>
        {
            if (RelicDisplay.Instance != null)
                RelicDisplay.Instance.RefreshDisplay();
        };
    }
}