using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropDownController : MonoBehaviour
{
    public TMP_Dropdown HeroDropDown;
    private string CurrentHeroID;
    public Button ComfirmButton;

    void Start()
    {
        HeroDropDown.options.Clear();
        HeroDropDown.options.Add(new TMP_Dropdown.OptionData("-- Select hero --"));
        var heroLibrary = HeroLiberary.Instance != null ? HeroLiberary.Instance.HeroDictionary : null;
        foreach (var hero in HeroDataBase.Instance.HeroDictionary.Values)
        {
            if (heroLibrary != null && !heroLibrary.ContainsKey(hero.ID))
                continue;
            HeroDropDown.options.Add(new HeroOptionData(hero.Name, hero.ID));
        }
        HeroDropDown.onValueChanged.AddListener(OnHeroSelected);
        ComfirmButton.onClick.AddListener(TriggerEventByHeroId);
        HeroDropDown.RefreshShownValue();
    }
    private void OnHeroSelected(int selectedIndex)
    {
        if (selectedIndex <= 0) return;
        if (HeroDropDown.options[selectedIndex] is HeroOptionData heroOption)
            CurrentHeroID = heroOption.HeroId;
    }

    /// <summary>
    /// Adds the selected hero as the single HumanPlayer in PlayerManager. Replaces any existing human from this panel.
    /// </summary>
    private void TriggerEventByHeroId()
    {
        if (string.IsNullOrEmpty(CurrentHeroID)) return;
        if (PlayerManager.Instance == null || HeroDataBase.Instance == null) return;
        if (!HeroDataBase.Instance.HeroDictionary.TryGetValue(CurrentHeroID, out var heroDefine))
            return;
        PlayerManager.Instance.RemoveAllHumanPlayers();
        PlayerManager.Instance.AddPlayer(isFriend: true, isHuman: true, heroDefine);
    }

    // Update is called once per frame
    void Update()
    {
        if (ComfirmButton != null)
            ComfirmButton.interactable = PlayerManager.Instance == null || PlayerManager.Instance.HumanPlayers.Count == 0;
    }
}


//???????????????????????????ID
public class HeroOptionData : TMP_Dropdown.OptionData
{
    public string HeroId { get; private set; }

    public HeroOptionData(string text, string heroId) : base(text)
    {
        HeroId = heroId;
    }
}