using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Roguelike battle outcome UI: shows <see cref="rewardPanel"/> after a win and <see cref="defeatPanel"/> after a loss.
/// Wire panels in the Inspector; they are hidden on awake unless you need otherwise.
/// </summary>
public class RougeBattleManager : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("Shown when the player wins the fight.")]
    public GameObject rewardPanel;

    [Tooltip("Shown when the player is defeated.")]
    public GameObject defeatPanel;

    [Tooltip("Button to return to the map.")]
    public Button ToMapButton;

    public TMP_Text CurrentHeroName;

    void Awake()
    {
        if (rewardPanel != null)
            rewardPanel.SetActive(false);
        if (defeatPanel != null)
            defeatPanel.SetActive(false);
        if (ToMapButton != null)
        {
            ToMapButton.gameObject.SetActive(true);
            ToMapButton.onClick.AddListener(OnToMapButtonClicked);
        }
    }

    void Start()
    {
        BattleManager.Instance.OnDefeated += OnBattleDefeated;
        BattleManager.Instance.OnWinning += OnBattleWinning;
        UpdateCurrentHeroNameText();
    }

    void OnDestroy()
    {
        if (BattleManager.Instance == null)
            return;
        BattleManager.Instance.OnDefeated -= OnBattleDefeated;
        BattleManager.Instance.OnWinning -= OnBattleWinning;
    }

    void OnBattleDefeated()
    {
        if (defeatPanel != null)
            defeatPanel.SetActive(true);
    }

    void OnBattleWinning()
    {
        if (rewardPanel != null)
            rewardPanel.SetActive(true);
    }

    void OnToMapButtonClicked()
    {
        const string mapSceneName = "RougeMap";
        MapCreator.SetMapRootActive(true);
        if (WorkingOn.Instance != null)
            WorkingOn.Instance.LoadScene(mapSceneName);
        else
            SceneManager.LoadScene(mapSceneName);
    }

    public void OnSwitchingHero()
    {
        var playerManager = PlayerManager.Instance;
        var rougePlayer = RougeManager.Instance?.rougePlayer;
        if (playerManager == null || playerManager.HumanPlayer == null || rougePlayer == null || rougePlayer.Heroes == null)
            return;

        var heroes = rougePlayer.Heroes;
        if (heroes.Count <= 1)
        {
            UpdateCurrentHeroNameText();
            return;
        }

        Hero currentHero = playerManager.HumanPlayer.hero;
        int currentIndex = -1;
        for (int i = 0; i < heroes.Count; i++)
        {
            if (ReferenceEquals(heroes[i], currentHero))
            {
                currentIndex = i;
                break;
            }
        }

        int nextIndex = (currentIndex + 1 + heroes.Count) % heroes.Count;
        var nextHero = heroes[nextIndex];
        if (nextHero == null)
            return;

        playerManager.HumanPlayer.SwitchHero(nextHero);
        UpdateCurrentHeroNameText();
    }

    private void UpdateCurrentHeroNameText()
    {
        if (CurrentHeroName == null)
            return;

        var currentHero = PlayerManager.Instance?.HumanPlayer?.hero;
        if (currentHero == null)
        {
            CurrentHeroName.text = "";
            return;
        }

        if (HeroDataBase.Instance != null &&
            HeroDataBase.Instance.HeroDictionary != null &&
            HeroDataBase.Instance.HeroDictionary.TryGetValue(currentHero.ID, out var heroDefine) &&
            heroDefine != null &&
            !string.IsNullOrEmpty(heroDefine.Name))
        {
            CurrentHeroName.text = heroDefine.Name;
            return;
        }

        CurrentHeroName.text = currentHero.ID ?? "";
    }


}
