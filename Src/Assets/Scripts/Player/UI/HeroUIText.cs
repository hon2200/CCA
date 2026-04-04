using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

/// <summary>
/// Binds a <see cref="Hero"/> to TMP fields: ID, HP, and skills as rich text (bold name, normal description, each skill on new lines).
/// </summary>
public class HeroUIText : MonoBehaviour
{
    private Hero hero;

    [Header("Simple fields")]
    [SerializeField] private TMP_Text idText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private HPFilling hpFilling;

    [Header("Skills")]
    [Tooltip("Rich Text should be enabled on this TMP for <b> tags.")]
    [SerializeField] private TMP_Text skillText;

    private Action<int, int, string> _hpChangedHandler;

    public Hero BoundHero => hero;

    /// <summary>Wire UI from a player’s hero (same source as <see cref="PlayerUIText"/>).</summary>
    public void Initialize(Player player) => Initialize(player?.hero);

    /// <summary>Wire UI to this hero (call when hero instance changes).</summary>
    public void Initialize(Hero newHero)
    {
        Unbind();
        hero = newHero;
        if (hero == null)
        {
            ClearUi();
            return;
        }

        if (idText != null)
            idText.text = hero.ID ?? "";

        SubscribeHp();
        UpdateHpDisplay();

        UpdateSkillsText();
    }

    private void OnDestroy() => Unbind();

    private void Unbind()
    {
        UnsubscribeHp();
    }

    private void ClearUi()
    {
        if (idText != null) idText.text = "";
        if (hpText != null) hpText.text = "";
        if (skillText != null) skillText.text = "";
        SetHpBar(0, 1);
    }

    private void SubscribeHp()
    {
        if (hero?.CurrentHP == null) return;
        _hpChangedHandler = (_, __, ___) => UpdateHpDisplay();
        hero.CurrentHP.OnValueChanged += _hpChangedHandler;
    }

    private void UnsubscribeHp()
    {
        if (hero?.CurrentHP != null && _hpChangedHandler != null)
            hero.CurrentHP.OnValueChanged -= _hpChangedHandler;
        _hpChangedHandler = null;
    }

    private void UpdateHpDisplay()
    {
        if (hero == null) return;
        int current = hero.CurrentHP != null ? hero.CurrentHP.Value : 0;
        int max = Mathf.Max(1, hero.MaxHP);
        if (hpText != null)
            hpText.text = $"{current}/{max}";
        SetHpBar(current, max);
    }

    private void SetHpBar(float currentHp, float maxHp)
    {
        if (hpFilling == null) return;
        float fill = maxHp > 0 ? currentHp / maxHp : 0f;
        hpFilling.MovingMask(fill);
    }

    private void UpdateSkillsText()
    {
        if (skillText == null) return;
        skillText.text = BuildSkillsRichText(hero?.skills);
    }

    /// <summary>
    /// Each skill: <c>&lt;b&gt;Name&lt;/b&gt;\nDescription</c>, then newline before the next skill.
    /// </summary>
    public static string BuildSkillsRichText(List<SkillDefine> skills)
    {
        if (skills == null || skills.Count == 0)
            return "";

        var sb = new StringBuilder();
        for (int i = 0; i < skills.Count; i++)
        {
            var skill = skills[i];
            if (skill == null)
                continue;

            string name = string.IsNullOrEmpty(skill.Name) ? skill.ID ?? "" : skill.Name;
            string desc = skill.Description ?? "";

            sb.Append("<b>");
            sb.Append(TmpEscapeForRichText(name));
            sb.Append("</b>\n");
            sb.Append(TmpEscapeForRichText(desc));

            if (i < skills.Count - 1)
                sb.Append('\n');
        }

        return sb.ToString();
    }

    /// <summary>Escape &lt; so user content cannot inject TMP tags (e.g. break out of &lt;b&gt;).</summary>
    private static string TmpEscapeForRichText(string s)
    {
        if (string.IsNullOrEmpty(s))
            return "";
        return s.Replace("\\", "\\\\").Replace("<", "\\<");
    }
}
