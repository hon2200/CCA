using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// UI for a single relic: image and count display. Acts like CardUI for relics.
/// Expects RuntimeRelic on the same GameObject (connects RelicDefine + RelicTemplete).
/// Reads template from RelicLiberary by relic ID; uses "Default" if no template is found.
/// </summary>
public class RelicUI : MonoBehaviour
{
    public SpriteRenderer sprite;

    public TextMeshProUGUI countText;

    private void Start()
    {
        InitializeFromLibrary();
    }

    /// <summary>
    /// Load image and count from RelicLiberary using RuntimeRelic.relicDefine.ID; fallback template ID is "Default".
    /// </summary>
    public void InitializeFromLibrary()
    {
        var runtimeRelic = GetComponent<RuntimeRelic>();
        if (runtimeRelic?.relicDefine == null) return;

        var library = RelicLiberary.Instance?.RelicDictionary;
        if (library == null) return;

        string id = runtimeRelic.relicDefine.ID;
        if (!library.TryGetValue(id, out var template) && !library.TryGetValue("Default", out template))
            return;

        if (sprite != null && template.Image != null)
            sprite.sprite = template.Image;
        runtimeRelic.relicTemplete = template;
        UpdateCount(runtimeRelic.relicDefine);
    }

    /// <summary>
    /// Update count UI from the RuntimeRelic's relicDefine.counts.
    /// </summary>
    public void UpdateCount()
    {
        var runtimeRelic = GetComponent<RuntimeRelic>();
        if (runtimeRelic?.relicDefine != null)
            UpdateCount(runtimeRelic.relicDefine);
        else if (countText != null)
            countText.text = "";
    }

    /// <summary>
    /// Update count UI from the given define's counts list.
    /// </summary>
    public void UpdateCount(RelicDefine define)
    {
        if (countText == null) return;
        if (define?.counts == null || define.counts.Count == 0)
        {
            countText.text = "";
            return;
        }
        countText.text = FormatCounts(define.counts);
    }

    private static string FormatCounts(List<int> counts)
    {
        if (counts == null || counts.Count == 0) return "";
        if (counts.Count == 1) return counts[0].ToString();
        return string.Join("/", counts);
    }
}
