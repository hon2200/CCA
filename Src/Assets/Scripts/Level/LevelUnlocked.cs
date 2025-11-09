using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Unlocked New Actions at the beginning of the battle
public class LevelUnlocked
{
    public GameObject UnlockedCardsPanel;
    public void ShowCards()
    {
        UnlockedCardsPanel.SetActive(true);
    }

    public void HideCards()
    {
        UnlockedCardsPanel.SetActive(false);
    }
}
