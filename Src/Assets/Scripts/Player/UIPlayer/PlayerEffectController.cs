using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectController : MonoBehaviour
{
    public Player player;
    public void Initialize()
    {
        player.status.HP.OnValueChanged += OnPlayerDamaged;
    }
    public void OnPlayerDamaged(int ori, int res, string message)
    {
        if (message == "Damage")
            EffectManager.Instance.Hit(player.gameObject);
    }
}
