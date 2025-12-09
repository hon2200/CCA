using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CheatManager : MonoSingleton<CheatManager>
{
    public Action OnResolution { get; set; }
    public void AddHP()
    {
        OnResolution += () => PlayerManager.Instance.HumanPlayer.status.HP.Heal(10);
    }
    public void AddAllHP()
    {
        OnResolution += () =>
        {
            foreach (var player in PlayerManager.Instance.Players.Values)
            {
                player.status.HP.Heal(10);
            }
        };

    }
    public void ClearBurn()
    {
        OnResolution += () =>
        {
            var buffSnaps = PlayerManager.Instance.HumanPlayer.status.buffs.ToList();
            foreach (var buff in buffSnaps)
            {
                if (buff.ID == "Burning")
                {
                    buff.Value -= 4;
                    if(buff.Value < 0)
                        buff.Value = 0;
                }
            }
            PlayerManager.Instance.HumanPlayer.status.resources.Sword.Lost(1);
        };

    }
    public void ClearBleeding()
    {
        OnResolution += () =>
        {
            var buffSnaps = PlayerManager.Instance.HumanPlayer.status.buffs.ToList();
            foreach (var buff in buffSnaps)
            {
                if (buff.ID == "Burning")
                {
                    buff.Value = 0;
                }
            }
            PlayerManager.Instance.HumanPlayer.status.resources.Sword.Lost(3);
        };
    }

}
