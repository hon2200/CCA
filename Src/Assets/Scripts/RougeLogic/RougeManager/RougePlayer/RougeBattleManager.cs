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
        if (WorkingOn.Instance != null)
            WorkingOn.Instance.LoadScene(mapSceneName);
        else
            SceneManager.LoadScene(mapSceneName);
    }
}
