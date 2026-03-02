using UnityEngine;
using UnityEngine.UI;

public class DeleteRelic : MonoBehaviour
{
    public void DeleteOneRelic()
    {
        var relics = RougeManager.Instance?.rougePlayer?.Relics;
        if (relics == null || relics.Count == 0) return;
        relics.RemoveAt(relics.Count - 1);
        RelicControlPanel.Instance.RefreshText();
    }

    private void Update()
    {
        var button = GetComponent<Button>();
        if (button == null) return;
        var relics = RougeManager.Instance?.rougePlayer?.Relics;
        bool hasRelics = relics != null && relics.Count > 0;
        button.interactable = hasRelics;
    }
}
