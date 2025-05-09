using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int ID_inGame { get; set; }
    public PlayerStatus status { get; set; }
    public PlayerAction action { get; set; }
    public PlayerUIText playerUIText;
    public void Initialize(int ID_inGame, PlayerStatus status, PlayerAction action)
    {
        this.ID_inGame = ID_inGame;
        this.status = status;
        this.action = action;
    }
}