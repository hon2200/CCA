using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// UI for a single relic: image and count display. Acts like CardUI for relics.
/// Expects RuntimeRelic on the same GameObject (connects RelicDefine + RelicTemplete).
/// </summary>
public class RelicUI : MonoBehaviour
{
    [Header("Image")]
    public SpriteRenderer sprite;

    [Header("Count display (optional)")]
    public TextMeshProUGUI countText;

    /// <summary>
    /// Initialize image from template. Call after RuntimeRelic.relicTemplete is set.
    /// </summary>
    public void Initialize(RelicTemplete template)
    {
        if (template == null) return;
        if (sprite != null && template.Image != null)
            sprite.sprite = template.Image;
        UpdateCount();
    }

    /// <summary>
    /// Initialize image from template and update count from define. Call when both RuntimeRelic.relicTemplete and relicDefine are set.
    /// </summary>
    public void Initialize(RelicTemplete template, RelicDefine define)
    {
        if (template == null) return;
        if (sprite != null && template.Image != null)
            sprite.sprite = template.Image;
        UpdateCount(define);
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
