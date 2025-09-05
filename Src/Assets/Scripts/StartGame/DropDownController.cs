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
        HeroDropDown.options.Add(new());
        foreach(var hero in HeroDataBase.Instance.HeroDictionary.Values)
        {
            HeroDropDown.options.Add(new HeroOptionData(hero.Name, hero.ID));
        }
        HeroDropDown.onValueChanged.AddListener(OnHeroSelected);
        ComfirmButton.onClick.AddListener(TriggerEventByHeroId);
    }
    private void OnHeroSelected(int selectedIndex)
    {
        if (HeroDropDown.options[selectedIndex] is HeroOptionData heroOption)
        {
            CurrentHeroID = heroOption.HeroId;
        }
    }

    private void TriggerEventByHeroId()
    {
        GameSetting.Instance.HeroIDDictionary.Add(CurrentHeroID);
        GameSetting.Instance.RefreshText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


//英雄选项类，包含名字（显示文本）和ID
public class HeroOptionData : TMP_Dropdown.OptionData
{
    public string HeroId { get; private set; }

    public HeroOptionData(string text, string heroId) : base(text)
    {
        HeroId = heroId;
    }
}