using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillManager:MonoSingleton<PlayerSkillManager>
{
    public GameObject SkillLoadingPanel;
    public Button AdvanceButton;
    public int skillSlots;
    public List<string> UnlockedSkills;
    public void Start()
    {
        AdvanceButton.onClick.AddListener(ToBattle);
    }
    private void Init()
    {
        LevelDefine currentLevel = LevelManager.Instance.GetCurrentLevel();
        skillSlots = currentLevel.PlayerSkillSlots;
        UnlockedSkills = currentLevel.GetAllUnlockedSkills();
    }
    public void OpenSkillPanel()
    {
        Init();
        SkillLoadingPanel.SetActive(true);
    }

    public void ToBattle()
    {
        SkillLoadingPanel.SetActive(false);
        BattleManager.Instance.OnStartGame("Level");
    }
}
