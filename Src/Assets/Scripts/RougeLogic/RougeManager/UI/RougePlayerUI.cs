using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI bridge for selecting hero IDs into RougePlayer and spawning a matching hero prefab.
/// </summary>
public class RougePlayerUI : MonoSingleton<RougePlayerUI>
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown heroDropdown;
    [SerializeField] private List<Button> SkillButtons;
    [SerializeField] private GameObject SkillButtonPrefab;
    [SerializeField] private GameObject SkillButtonPanel;
    [SerializeField] private float SkillButtonSpacing = 20f;
    [SerializeField] private TMP_Text SkillText;
    [SerializeField] private TMP_Text HPText;

    private string currentHeroID;

    public void Initialize()
    {
        BuildHeroDropdown();

        if (heroDropdown != null)
            heroDropdown.onValueChanged.AddListener(OnHeroSelected);
    }

    private void BuildHeroDropdown()
    {
        if (heroDropdown == null)
            return;

        heroDropdown.options.Clear();
        heroDropdown.options.Add(new TMP_Dropdown.OptionData("-- Select hero --"));

        var rougePlayer = RougeManager.Instance != null ? RougeManager.Instance.rougePlayer : null;
        var heroes = rougePlayer != null ? rougePlayer.Heroes : null;

        if (heroes == null || heroes.Count == 0 ||
            HeroDataBase.Instance == null || HeroDataBase.Instance.HeroDictionary == null)
        {
            heroDropdown.value = 0;
            heroDropdown.RefreshShownValue();
            return;
        }

        var idOccurrence = new Dictionary<string, int>();
        foreach (var hero in heroes)
        {
            if (hero == null || string.IsNullOrEmpty(hero.ID))
                continue;

            if (!HeroDataBase.Instance.HeroDictionary.TryGetValue(hero.ID, out var heroDefine) || heroDefine == null)
                continue;

            if (!idOccurrence.TryGetValue(hero.ID, out int n))
                n = 0;
            idOccurrence[hero.ID] = n + 1;

            string label = n == 0 ? heroDefine.Name : $"{heroDefine.Name} ({n + 1})";
            heroDropdown.options.Add(new RougeHeroOptionData(label, hero, heroDefine));
        }

        heroDropdown.value = 0;
        heroDropdown.RefreshShownValue();
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
