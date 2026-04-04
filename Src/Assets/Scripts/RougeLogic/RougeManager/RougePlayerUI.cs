using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI bridge for selecting hero IDs into RougePlayer and spawning a matching hero prefab.
/// </summary>
public class RougePlayerUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown heroDropdown;
    [SerializeField] private Button confirmButton;

    [Header("Spawn")]
    [SerializeField] private Transform heroSpawnParent;
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private bool clearSpawnParentBeforeSpawn = true;

    private string currentHeroID;

    private void Start()
    {
        BuildHeroDropdown();

        if (heroDropdown != null)
            heroDropdown.onValueChanged.AddListener(OnHeroSelected);
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmHero);
    }

    private void BuildHeroDropdown()
    {
        if (heroDropdown == null)
            return;

        heroDropdown.options.Clear();
        heroDropdown.options.Add(new TMP_Dropdown.OptionData("-- Select hero --"));

        if (HeroDataBase.Instance == null || HeroDataBase.Instance.HeroDictionary == null)
        {
            heroDropdown.RefreshShownValue();
            return;
        }

        var heroLibrary = HeroLiberary.Instance != null ? HeroLiberary.Instance.HeroDictionary : null;
        foreach (var hero in HeroDataBase.Instance.HeroDictionary.Values)
        {
            if (hero == null)
                continue;
            if (heroLibrary != null && !heroLibrary.ContainsKey(hero.ID))
                continue;

            heroDropdown.options.Add(new HeroOptionData(hero.Name, hero.ID));
        }

        heroDropdown.value = 0;
        heroDropdown.RefreshShownValue();
    }

    private void OnHeroSelected(int selectedIndex)
    {
        currentHeroID = null;

        if (heroDropdown == null || selectedIndex <= 0 || selectedIndex >= heroDropdown.options.Count)
            return;

        if (heroDropdown.options[selectedIndex] is HeroOptionData heroOption)
            currentHeroID = heroOption.HeroId;
    }

    private void OnConfirmHero()
    {
        if (string.IsNullOrEmpty(currentHeroID))
            return;
        if (RougeManager.Instance == null || RougeManager.Instance.rougePlayer == null)
            return;
        if (HeroDataBase.Instance == null || HeroDataBase.Instance.HeroDictionary == null)
            return;

        if (!HeroDataBase.Instance.HeroDictionary.TryGetValue(currentHeroID, out var heroDefine))
            return;

        var rougePlayer = RougeManager.Instance.rougePlayer;
        var recruitedHero = rougePlayer.RecruitHero(heroDefine);
        if (recruitedHero == null)
            return;

        SpawnSelectedHero(heroDefine);
    }

    private void SpawnSelectedHero(HeroDefine heroDefine)
    {
        if (heroDefine == null || heroPrefab == null || heroSpawnParent == null)
            return;

        if (clearSpawnParentBeforeSpawn)
        {
            for (int i = heroSpawnParent.childCount - 1; i >= 0; i--)
                Destroy(heroSpawnParent.GetChild(i).gameObject);
        }

        var heroObject = Instantiate(heroPrefab, heroSpawnParent);
        heroObject.transform.localPosition = Vector3.zero;

        var runtimeHero = heroObject.GetComponent<RuntimeHero>();
        if (runtimeHero == null)
            return;

        runtimeHero.heroDefine = heroDefine;
        if (HeroLiberary.Instance != null && HeroLiberary.Instance.HeroDictionary != null)
            HeroLiberary.Instance.HeroDictionary.TryGetValue(heroDefine.ID, out runtimeHero.heroTemplete);
    }
}
