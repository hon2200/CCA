using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

/// <summary>
/// Lists <see cref="RougePlayer.Heroes"/> in one TMP block: name (left), dot leaders, current/max HP (right), one hero per line.
/// </summary>
public class RougePlayerUIText : MonoBehaviour
{
    [SerializeField] private TMP_Text heroesText;
    [Tooltip("If true, calls Initialize from RougeManager when this component is enabled.")]
    [SerializeField] private bool bindFromRougeManagerOnEnable = true;

    private RougePlayer rougePlayer;

    private readonly List<(Hero hero, Action<int, int, string> handler)> _hpSubscriptions = new List<(Hero, Action<int, int, string>)>();

    public RougePlayer BoundRougePlayer => rougePlayer;

    private void OnEnable()
    {
        if (bindFromRougeManagerOnEnable && rougePlayer == null)
            Initialize(RougeManager.Instance != null ? RougeManager.Instance.rougePlayer : null);
        else if (rougePlayer != null)
            RefreshDisplay();
    }

    private void OnDestroy() => Unbind();

    /// <summary>Bind to the current singleton run player.</summary>
    public void Initialize() => Initialize(RougeManager.Instance != null ? RougeManager.Instance.rougePlayer : null);

    public void Initialize(RougePlayer player)
    {
        Unbind();
        rougePlayer = player;
        if (rougePlayer == null)
        {
            ClearText();
            return;
        }

        SubscribeAllHeroHp();
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (heroesText == null)
            return;

        if (rougePlayer?.Heroes == null || rougePlayer.Heroes.Count == 0)
        {
            heroesText.text = "";
            return;
        }

        var sb = new StringBuilder();
        bool first = true;
        for (int i = 0; i < rougePlayer.Heroes.Count; i++)
        {
            var hero = rougePlayer.Heroes[i];
            if (hero == null)
                continue;

            if (!first)
                sb.Append('\n');
            first = false;

            string name = GetHeroDisplayName(hero);
            string hpPart = FormatHp(hero);
            sb.Append(FormatHeroLine(heroesText, name, hpPart));
        }

        heroesText.text = sb.ToString();
    }

    private void ClearText()
    {
        if (heroesText != null)
            heroesText.text = "";
    }

    private void Unbind()
    {
        foreach (var (hero, handler) in _hpSubscriptions)
        {
            if (hero?.CurrentHP != null && handler != null)
                hero.CurrentHP.OnValueChanged -= handler;
        }

        _hpSubscriptions.Clear();
        rougePlayer = null;
    }

    private void SubscribeAllHeroHp()
    {
        if (rougePlayer?.Heroes == null)
            return;

        foreach (var hero in rougePlayer.Heroes)
        {
            if (hero?.CurrentHP == null)
                continue;

            Action<int, int, string> handler = (_, __, ___) => RefreshDisplay();
            hero.CurrentHP.OnValueChanged += handler;
            _hpSubscriptions.Add((hero, handler));
        }
    }

    public static string GetHeroDisplayName(Hero hero)
    {
        if (hero == null)
            return "";

        if (HeroDataBase.Instance != null
            && HeroDataBase.Instance.HeroDictionary != null
            && HeroDataBase.Instance.HeroDictionary.TryGetValue(hero.ID, out var def)
            && def != null
            && !string.IsNullOrEmpty(def.Name))
            {
                return def.Name;
            }

        return string.IsNullOrEmpty(hero.ID) ? "?" : hero.ID;
    }

    public static string FormatHp(Hero hero)
    {
        if (hero == null)
            return "0/0";

        int max = hero.MaxHP;
        if (max <= 0
            && HeroDataBase.Instance != null
            && HeroDataBase.Instance.HeroDictionary != null
            && HeroDataBase.Instance.HeroDictionary.TryGetValue(hero.ID, out var def)
            && def != null)
        {
            max = def.MaxHP;
        }

        if (max <= 0)
            max = 1;

        int current = hero.CurrentHP != null ? hero.CurrentHP.Value : max;
        return $"{current}/{max}";
    }

    /// <summary>
    /// Single line: left name, period leaders filling to the right column, HP text flush right within the text area width.
    /// </summary>
    public static string FormatHeroLine(TMP_Text tmp, string name, string hpText)
    {
        if (tmp == null)
            return $"{name} ...... {hpText}";

        float lineWidth = tmp.rectTransform.rect.width - tmp.margin.x - tmp.margin.z;
        if (lineWidth <= 1f)
            lineWidth = 400f;

        float nameW = tmp.GetPreferredValues(name).x;
        float hpW = tmp.GetPreferredValues(hpText).x;
        float dotW = tmp.GetPreferredValues(".").x;
        if (dotW < 0.001f)
            dotW = 6f;

        float gap = lineWidth - nameW - hpW - 4f;
        int dotCount = Mathf.Clamp(Mathf.FloorToInt(gap / dotW), 3, 120);
        return name + new string('.', dotCount) + hpText;
    }
}
