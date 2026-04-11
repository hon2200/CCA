using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// UI bridge for selecting hero IDs into RougePlayer and spawning a matching hero prefab.
/// </summary>
public class RougePlayerUI : MonoSingleton<RougePlayerUI>
{
    private const string RougeMapSceneName = "RougeMap";

    [Header("UI")]
    [SerializeField] private TMP_Dropdown heroDropdown;
    [SerializeField] private List<Button> SkillButtons;
    [SerializeField] private GameObject SkillButtonPrefab;
    [SerializeField] private GameObject SkillButtonPanel;
    [SerializeField] private float SkillButtonSpacing = 20f;
    [SerializeField] private TMP_Text SkillText;
    [SerializeField] private TMP_Text HPText;
    [SerializeField] private TMP_Text GoldText;

    [SerializeField] private GameObject HeroPanel;

    private string currentHeroID;

    private GoldAttribute _boundGold;
    private Action<int, int, string> _onGoldChanged;
    private Coroutine _bindGoldAfterLoadRoutine;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnRougeMapSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnRougeMapSceneLoaded;
        if (_bindGoldAfterLoadRoutine != null)
        {
            StopCoroutine(_bindGoldAfterLoadRoutine);
            _bindGoldAfterLoadRoutine = null;
        }
        UnbindGoldText();
    }

    private void OnRougeMapSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != RougeMapSceneName)
            return;
        if (_bindGoldAfterLoadRoutine != null)
            StopCoroutine(_bindGoldAfterLoadRoutine);
        _bindGoldAfterLoadRoutine = StartCoroutine(BindGoldTextAfterMapReady());
    }

    /// <summary>
    /// <see cref="SceneManager.sceneLoaded"/> runs before <c>Start</c>; <see cref="RougeManager"/> creates
    /// <see cref="RougeManager.rougePlayer"/> in <c>Start</c>, so we wait until gold exists then bind.
    /// </summary>
    private IEnumerator BindGoldTextAfterMapReady()
    {
        const int maxFrames = 30;
        for (int i = 0; i < maxFrames; i++)
        {
            var gold = RougeManager.Instance?.rougePlayer?.gold;
            if (gold != null && GoldText != null)
                break;
            yield return null;
        }
        _bindGoldAfterLoadRoutine = null;
        BindGoldText();
    }

    public void Initialize()
    {
        var heroes = RougeManager.Instance?.rougePlayer?.Heroes;
        if (heroes != null)
        {
            heroes.OnListChanged += (List<Hero> newHeroes, string message)
                => { BuildRougeHeroDropDown(); };
        }
        BuildRougeHeroDropDown();
        BindGoldText();

        if (heroDropdown != null)
            heroDropdown.onValueChanged.AddListener(OnHeroSelected);
    }

    /// <summary>Re-reads current gold and re-subscribes; safe to call after map load or run reset.</summary>
    public void RefreshGoldText() => BindGoldText();
    public void OpenAndCloseHeroPanel()
    {
        if(HeroPanel.activeSelf)
        {
            HeroPanel.SetActive(false);
        }
        else
        {
            HeroPanel.SetActive(true);
        }
    }
    public void SetHeroPanelActive(bool active)
    {
        if (HeroPanel != null)
            HeroPanel.SetActive(active);
    }

    public Hero GetSelectedRougeHero()
    {
        if (heroDropdown == null)
            return null;

        int selectedIndex = heroDropdown.value;
        if (selectedIndex <= 0 || selectedIndex >= heroDropdown.options.Count)
            return null;

        if (heroDropdown.options[selectedIndex] is RougeHeroOptionData opt)
            return opt.Hero;

        return null;
    }
    private void UnbindGoldText()
    {
        if (_boundGold != null && _onGoldChanged != null)
            _boundGold.OnValueChanged -= _onGoldChanged;
        _boundGold = null;
        _onGoldChanged = null;
    }

    private void BindGoldText()
    {
        UnbindGoldText();
        if (GoldText == null)
            return;

        var gold = RougeManager.Instance?.rougePlayer?.gold;
        if (gold == null)
            return;

        _boundGold = gold;
        _onGoldChanged = OnGoldValueChanged;
        gold.OnValueChanged += _onGoldChanged;
        GoldText.text = gold.Value.ToString();
    }

    private void OnGoldValueChanged(int oldVal, int newVal, string message)
    {
        if (GoldText != null)
            GoldText.text = newVal.ToString();
    }

    public void BuildRougeHeroDropDown()
    {
        BuildHeroDropdownInternal(heroDropdown, null, "-- Select hero --");
    }

    private void BuildHeroDropdownInternal(TMP_Dropdown dropdown, Hero excludedHero, string defaultLabel)
    {
        if (dropdown == null)
            return;

        dropdown.options.Clear();
        dropdown.options.Add(new TMP_Dropdown.OptionData(defaultLabel));

        var rougePlayer = RougeManager.Instance != null ? RougeManager.Instance.rougePlayer : null;
        var heroes = rougePlayer != null ? rougePlayer.Heroes : null;

        if (heroes == null || heroes.Count == 0 ||
            HeroDataBase.Instance == null || HeroDataBase.Instance.HeroDictionary == null)
        {
            dropdown.value = 0;
            dropdown.RefreshShownValue();
            return;
        }

        var idOccurrence = new Dictionary<string, int>();
        foreach (var hero in heroes)
        {
            if (hero == null || string.IsNullOrEmpty(hero.ID))
                continue;
            if (hero == excludedHero)
                continue;

            if (!HeroDataBase.Instance.HeroDictionary.TryGetValue(hero.ID, out var heroDefine) || heroDefine == null)
                continue;

            if (!idOccurrence.TryGetValue(hero.ID, out int n))
                n = 0;
            idOccurrence[hero.ID] = n + 1;

            string label = n == 0 ? heroDefine.Name : $"{heroDefine.Name} ({n + 1})";
            dropdown.options.Add(new RougeHeroOptionData(label, hero, heroDefine));
        }

        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }

    private void OnHeroSelected(int selectedIndex)
    {
        currentHeroID = null;

        if (heroDropdown == null || selectedIndex < 0 || selectedIndex >= heroDropdown.options.Count)
            return;

        if (selectedIndex <= 0)
        {
            ClearSkillButtonPanel();
            if (HPText != null)
                HPText.text = "";
            if (SkillText != null)
                SkillText.text = "";
            return;
        }

        if (heroDropdown.options[selectedIndex] is not RougeHeroOptionData opt || opt.HeroDefine == null)
        {
            ClearSkillButtonPanel();
            if (HPText != null)
                HPText.text = "";
            if (SkillText != null)
                SkillText.text = "";
            return;
        }

        currentHeroID = opt.HeroDefine.ID;

        var skills = GetSkillsFromOption(opt);
        RebuildSkillButtons(skills);
        UpdateHpTextFromOption(opt);

        if (SkillText != null)
        {
            if (skills != null && skills.Count > 0 && skills[0] != null)
                SkillText.text = skills[0].Description ?? "";
            else
                SkillText.text = "";
        }

    }

    private static List<SkillDefine> GetSkillsFromOption(RougeHeroOptionData opt)
    {
        var list = new List<SkillDefine>();

        if (opt.Hero?.skills != null)
        {
            foreach (var s in opt.Hero.skills)
            {
                if (s != null)
                    list.Add(s);
            }
        }

        if (list.Count > 0)
            return list;

        var define = opt.HeroDefine;
        if (define.SkillIDList == null || SkillDatabase.Instance == null)
            return list;

        foreach (var skillId in define.SkillIDList)
        {
            if (string.IsNullOrEmpty(skillId))
                continue;
            if (SkillDatabase.Instance.skillDic.TryGetValue(skillId, out var skill) && skill != null)
                list.Add(skill.Clone());
        }

        return list;
    }

    private void UpdateHpTextFromOption(RougeHeroOptionData opt)
    {
        if (HPText == null || opt.HeroDefine == null)
            return;

        var define = opt.HeroDefine;
        int max = Mathf.Max(1, define.MaxHP);
        int current = max;

        Hero h = opt.Hero;
        if (h != null)
        {
            if (h.MaxHP > 0)
                max = Mathf.Max(1, h.MaxHP);
            if (h.CurrentHP != null)
                current = h.CurrentHP.Value;
            else
                current = max;
        }

        HPText.text = $"{current}/{max}";
    }

    private void RebuildSkillButtons(List<SkillDefine> skills)
    {
        ClearSkillButtonPanel();

        if (SkillButtonPrefab == null || SkillButtonPanel == null || skills == null || skills.Count == 0)
            return;

        for (int i = 0; i < skills.Count; i++)
        {
            var skill = skills[i];
            if (skill == null)
                continue;

            var instance = Instantiate(SkillButtonPrefab, SkillButtonPanel.transform, false);
            var rect = instance.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 1f);
                rect.anchorMax = new Vector2(0.5f, 1f);
                rect.pivot = new Vector2(0.5f, 1f);
                rect.anchoredPosition = new Vector2(0f, -i * SkillButtonSpacing);
            }

            SetSkillButtonLabel(instance, skill);
            var button = instance.GetComponent<Button>();
            if (button != null)
            {
                var captured = skill;
                button.onClick.AddListener(() =>
                {
                    if (SkillText != null && captured != null)
                        SkillText.text = captured.Description ?? "";
                });
            }
        }
    }

    private static void SetSkillButtonLabel(GameObject instance, SkillDefine skill)
    {
        string label = string.IsNullOrEmpty(skill.Name) ? (skill.ID ?? "") : skill.Name;

        var skillUi = instance.GetComponentInChildren<SkillUIText>(true);
        if (skillUi != null && skillUi.text != null)
        {
            skillUi.text.text = label;
            return;
        }

        var tmp = instance.GetComponentInChildren<TMP_Text>(true);
        if (tmp != null)
            tmp.text = label;
    }

    private void ClearSkillButtonPanel()
    {
        if (SkillButtonPanel == null)
            return;

        for (int i = SkillButtonPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(SkillButtonPanel.transform.GetChild(i).gameObject);
    }
}

/// <summary>
/// Dropdown entry for one <see cref="Hero"/> on the roguelike roster and its <see cref="HeroDefine"/>.
/// </summary>
public class RougeHeroOptionData : TMP_Dropdown.OptionData
{
    public Hero Hero { get; private set; }
    public HeroDefine HeroDefine { get; private set; }

    public RougeHeroOptionData(string text, Hero hero, HeroDefine heroDefine) : base(text)
    {
        Hero = hero;
        HeroDefine = heroDefine;
    }
}
