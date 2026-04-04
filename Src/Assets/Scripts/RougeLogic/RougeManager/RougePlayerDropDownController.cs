using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Fills a <see cref="TMP_Dropdown"/> with <see cref="RougePlayer.Heroes"/>; each option shows
/// the same name / dot leaders / current–max HP layout as <see cref="RougePlayerUIText"/>.
/// </summary>
public class RougePlayerDropDownController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown heroDropdown;
    [SerializeField] private Button confirmButton;
    [Tooltip("Invoked when Confirm is clicked and a hero is selected. Read SelectedHero on this component in your handler.")]
    [SerializeField] private UnityEvent onConfirm;

    [Tooltip("If true, calls Initialize from RougeManager when this component is enabled and not yet bound.")]
    [SerializeField] private bool bindFromRougeManagerOnEnable = true;

    private RougePlayer rougePlayer;
    private readonly List<(Hero hero, Action<int, int, string> handler)> _hpSubscriptions = new List<(Hero, Action<int, int, string>)>();
    private bool _listenersRegistered;

    public Hero SelectedHero { get; private set; }

    public RougePlayer BoundRougePlayer => rougePlayer;

    private void Awake() => RegisterListeners();

    private void OnEnable()
    {
        if (bindFromRougeManagerOnEnable && rougePlayer == null)
            Initialize(RougeManager.Instance != null ? RougeManager.Instance.rougePlayer : null);
        else if (rougePlayer != null)
            RefreshOptionLabels();
    }

    private void OnDestroy() => Unbind();

    private void Update()
    {
        if (confirmButton != null)
            confirmButton.interactable = SelectedHero != null;
    }

    private void RegisterListeners()
    {
        if (_listenersRegistered || heroDropdown == null)
            return;

        heroDropdown.onValueChanged.AddListener(OnHeroSelected);
        _listenersRegistered = true;

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    /// <summary>Bind to the current singleton run player.</summary>
    public void Initialize() => Initialize(RougeManager.Instance != null ? RougeManager.Instance.rougePlayer : null);

    public void Initialize(RougePlayer player)
    {
        UnbindHpOnly();
        rougePlayer = player;

        if (heroDropdown == null)
            return;

        if (rougePlayer == null || rougePlayer.Heroes == null || rougePlayer.Heroes.Count == 0)
        {
            heroDropdown.ClearOptions();
            heroDropdown.options.Add(new TMP_Dropdown.OptionData("-- Select hero --"));
            heroDropdown.value = 0;
            heroDropdown.RefreshShownValue();
            SelectedHero = null;
            return;
        }

        BuildDropdownOptions();
        SubscribeAllHeroHp();
        heroDropdown.SetValueWithoutNotify(0);
        SelectedHero = null;
        heroDropdown.RefreshShownValue();
    }

    /// <summary>Rebuild options from <see cref="RougePlayer.Heroes"/> (e.g. after recruiting or dismissing).</summary>
    public void RebuildFromRougePlayer()
    {
        Hero keep = SelectedHero;
        UnbindHpOnly();

        if (heroDropdown == null)
            return;

        if (rougePlayer == null || rougePlayer.Heroes == null || rougePlayer.Heroes.Count == 0)
        {
            heroDropdown.ClearOptions();
            heroDropdown.options.Add(new TMP_Dropdown.OptionData("-- Select hero --"));
            heroDropdown.SetValueWithoutNotify(0);
            SelectedHero = null;
            heroDropdown.RefreshShownValue();
            return;
        }

        BuildDropdownOptions();
        SubscribeAllHeroHp();

        int newIndex = 0;
        if (keep != null)
        {
            for (int i = 1; i < heroDropdown.options.Count; i++)
            {
                if (heroDropdown.options[i] is RougeHeroOptionData ro && ro.Hero == keep)
                {
                    newIndex = i;
                    break;
                }
            }
        }

        heroDropdown.SetValueWithoutNotify(newIndex);
        OnHeroSelected(newIndex);
        heroDropdown.RefreshShownValue();
    }

    private void BuildDropdownOptions()
    {
        heroDropdown.ClearOptions();
        var list = new List<TMP_Dropdown.OptionData> { new TMP_Dropdown.OptionData("-- Select hero --") };
        TMP_Text metrics = GetMetricsTmp();

        foreach (var hero in rougePlayer.Heroes)
        {
            if (hero == null)
                continue;

            string name = RougePlayerUIText.GetHeroDisplayName(hero);
            string hpPart = RougePlayerUIText.FormatHp(hero);
            string line = RougePlayerUIText.FormatHeroLine(metrics, name, hpPart);
            list.Add(new RougeHeroOptionData(line, hero));
        }

        heroDropdown.AddOptions(list);
    }

    private TMP_Text GetMetricsTmp()
    {
        if (heroDropdown == null)
            return null;
        if (heroDropdown.itemText != null)
            return heroDropdown.itemText;
        return heroDropdown.captionText;
    }

    private void RefreshOptionLabels()
    {
        if (heroDropdown == null || rougePlayer?.Heroes == null)
            return;

        TMP_Text metrics = GetMetricsTmp();
        int saved = heroDropdown.value;

        foreach (var opt in heroDropdown.options)
        {
            if (opt is RougeHeroOptionData ro && ro.Hero != null)
            {
                string name = RougePlayerUIText.GetHeroDisplayName(ro.Hero);
                string hpPart = RougePlayerUIText.FormatHp(ro.Hero);
                opt.text = RougePlayerUIText.FormatHeroLine(metrics, name, hpPart);
            }
        }

        heroDropdown.RefreshShownValue();
        if (saved >= 0 && saved < heroDropdown.options.Count)
            heroDropdown.SetValueWithoutNotify(saved);
    }

    private void OnHeroSelected(int index)
    {
        if (index <= 0 || heroDropdown == null || index >= heroDropdown.options.Count)
        {
            SelectedHero = null;
            return;
        }

        if (heroDropdown.options[index] is RougeHeroOptionData heroOption)
            SelectedHero = heroOption.Hero;
        else
            SelectedHero = null;
    }

    private void OnConfirmClicked()
    {
        if (SelectedHero == null)
            return;
        onConfirm?.Invoke();
    }

    private void SubscribeAllHeroHp()
    {
        if (rougePlayer?.Heroes == null)
            return;

        foreach (var hero in rougePlayer.Heroes)
        {
            if (hero?.CurrentHP == null)
                continue;

            Action<int, int, string> handler = (_, __, ___) => RefreshOptionLabels();
            hero.CurrentHP.OnValueChanged += handler;
            _hpSubscriptions.Add((hero, handler));
        }
    }

    private void UnbindHpOnly()
    {
        foreach (var (hero, handler) in _hpSubscriptions)
        {
            if (hero?.CurrentHP != null && handler != null)
                hero.CurrentHP.OnValueChanged -= handler;
        }

        _hpSubscriptions.Clear();
    }

    private void Unbind()
    {
        UnbindHpOnly();
        rougePlayer = null;
        SelectedHero = null;
    }
}

/// <summary>Dropdown option carrying the <see cref="Hero"/> instance for selection callbacks.</summary>
public class RougeHeroOptionData : TMP_Dropdown.OptionData
{
    public Hero Hero { get; }

    public RougeHeroOptionData(string text, Hero hero) : base(text)
    {
        Hero = hero;
    }
}
