using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Clicking a monster option adds it directly to PlayerManager as an enemy.
/// </summary>
public class MonsterAddController : MonoBehaviour
{
    public TMP_Dropdown MonsterDropDown;
    public Button ConfirmButton;
    private string _currentMonsterID;

    void Start()
    {
        if (MonsterDropDown == null) return;
        MonsterDropDown.options.Clear();
        MonsterDropDown.options.Add(new TMP_Dropdown.OptionData("-- Select monster --"));
        if (HeroDataBase.Instance != null && HeroDataBase.Instance.EnemyDictionary != null)
        {
            var enemyLibrary = EnemyLiberary.Instance != null ? EnemyLiberary.Instance.EnemyDictionary : null;
            foreach (var enemy in HeroDataBase.Instance.EnemyDictionary.Values)
            {
                if (enemyLibrary != null && !enemyLibrary.ContainsKey(enemy.ID))
                    continue;
                MonsterDropDown.options.Add(new MonsterOptionData(enemy.Name, enemy.ID));
            }
        }
        MonsterDropDown.onValueChanged.AddListener(OnMonsterSelected);
        MonsterDropDown.RefreshShownValue();
        if (ConfirmButton != null)
            ConfirmButton.onClick.AddListener(AddSelectedMonster);
    }

    private void OnMonsterSelected(int selectedIndex)
    {
        if (selectedIndex <= 0) return;
        if (MonsterDropDown.options[selectedIndex] is MonsterOptionData option)
            _currentMonsterID = option.EnemyId;
    }

    /// <summary>
    /// Call from a button click: adds the currently selected monster to PlayerManager.
    /// </summary>
    public void AddSelectedMonster()
    {
        if (string.IsNullOrEmpty(_currentMonsterID)) return;
        if (PlayerManager.Instance == null || HeroDataBase.Instance == null) return;
        if (!HeroDataBase.Instance.EnemyDictionary.TryGetValue(_currentMonsterID, out var enemyDefine))
            return;
        if (PlayerManager.Instance.EnemyReachMaxNumber())
            return;
        PlayerManager.Instance.AddPlayer(isFriend: false, isHuman: false, enemyDefine);
    }
}

public class MonsterOptionData : TMP_Dropdown.OptionData
{
    public string EnemyId { get; private set; }

    public MonsterOptionData(string text, string enemyId) : base(text)
    {
        EnemyId = enemyId;
    }
}
