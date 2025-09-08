using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    public GameObject Win;
    public GameObject Defeat;
    public GameObject block;
    public TMP_Text blockText;
    public void OpenWhenWinning()
    {
        block.SetActive(true);
        blockText.text = "You Win!";
        Win.SetActive(true);
        Defeat.SetActive(false);
    }
    public void OpenWhenLosing()
    {
        block.SetActive(true);
        blockText.text = "You Are Defeated!";
        Win.SetActive(false);
        Defeat.SetActive(true);
    }
    public void CloseWhenRewarding()
    {
        gameObject.SetActive(false);
    }
}
