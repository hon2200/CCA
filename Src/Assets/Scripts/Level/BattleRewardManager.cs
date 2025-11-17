using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleRewardManager : MonoSingleton<BattleRewardManager>
{
    public GameObject BattleRewardPanel;
    public Button AdvanceButton;
    public TextMeshProUGUI skillText;
    public void Start()
    {
        //进入下一关
        AdvanceButton.onClick.AddListener(CloseBattleReward);
        AdvanceButton.onClick.AddListener(LevelManager.Instance.Advance);
    }

    public void OpenBattleReward()
    {
        BattleRewardPanel.SetActive(true);
        skillText.text = "";
        foreach (var skill in LevelManager.Instance.GetCurrentLevel().UnlockedSkillFinal)
            skillText.text += $"获得新技能 {skill} \n";
        skillText.text += $"获得新技能槽 {LevelManager.Instance.GetNextLevel().GetIncreasedSkillSlots()} \n";
        skillText.text += $"HP上限提升{LevelManager.Instance.GetNextLevel().GetIncreasedMaxHP()}\n";
    }

    public void CloseBattleReward()
    {
        BattleRewardPanel.SetActive(false);
    }

}
