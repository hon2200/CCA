using System.Collections.Generic;
using TMPro;

public class RelicControlPanel : MonoSingleton<RelicControlPanel>
{
    public TMP_Text SelectedRelicText;

    private new void Awake()
    {
        base.Awake();
        RefreshText();
    }

    /// <summary>
    /// Relic list is stored in RougeManager.Instance.rougePlayer.Relics.
    /// </summary>
    public void RefreshText()
    {
        SelectedRelicText.text = "";
        var relics = RougeManager.Instance?.rougePlayer?.Relics;
        if (relics == null || relics.Count == 0)
            SelectedRelicText.text = "No Relics";
        else if (relics.Count >= 10)
            SelectedRelicText.text = "Too Many";
        else
        {
            foreach (var relic in relics)
            {
                if (relic != null)
                    SelectedRelicText.text += relic.Name + "\n";
                else
                    SelectedRelicText.text += "Unknown\n";
            }
        }
    }
}
