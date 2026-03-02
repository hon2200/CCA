using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicDropDownController : MonoBehaviour
{
    public TMP_Dropdown RelicDropDown;
    private string CurrentRelicID;
    public Button ConfirmButton;

    private void Start()
    {

        RelicDropDown.options.Add(new TMP_Dropdown.OptionData());
        foreach (var relic in RelicDatabase.Instance.RelicDictionary.Values)
        {
            RelicDropDown.options.Add(new RelicOptionData(relic.Name, relic.ID));
        }
        RelicDropDown.onValueChanged.AddListener(OnRelicSelected);
        ConfirmButton.onClick.AddListener(AddSelectedRelic);
    }

    private void OnRelicSelected(int selectedIndex)
    {

        if (RelicDropDown.options[selectedIndex] is RelicOptionData relicOption)
        {
            CurrentRelicID = relicOption.RelicId;
        }
    }

    private void AddSelectedRelic()
    {
        if (string.IsNullOrEmpty(CurrentRelicID)) return;

        
        var rougePlayer = RougeManager.Instance.rougePlayer;

        if (rougePlayer.Relics != null && rougePlayer.Relics.Count >= 10) return;

        RelicDefine relic = RelicDatabase.Instance.GetRelic(CurrentRelicID);
        Debug.Assert(relic != null, $"[RelicDropDownController] GetRelic returned null for ID: {CurrentRelicID}");

        if (rougePlayer.GetRelic(relic))
        {
            RelicControlPanel.Instance.RefreshText();
        }
    }
}

/// <summary>
/// Option data for relic dropdown: display text (Name) and RelicId.
/// </summary>
public class RelicOptionData : TMP_Dropdown.OptionData
{
    public string RelicId { get; private set; }

    public RelicOptionData(string text, string relicId) : base(text)
    {
        RelicId = relicId;
    }
}
