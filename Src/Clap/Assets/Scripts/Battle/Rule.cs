using UnityEngine;
using UnityEngine.UI;

public class Rule : MonoSingleton<Rule>
{
    public delegate void RoundChangedHandler();
    public static event RoundChangedHandler OnRoundChanged;
    [SerializeField] private float RoundDura = 60f; // 回合持续时间
    [SerializeField] private float RoundAgain = 70f; // 间隔时间

    private float time=0f;
    private bool isRoundEnd = false;

    public Image clockImage;

    public void  Start()
    {
        CardFiltering.Instance.Set();
    }
    private void FixedUpdate()
    {
        if (Battle.Instance.AllReady())
        {
            time = RoundDura;
        }
        time += Time.fixedDeltaTime;
        clockImage.fillAmount=1-time/RoundDura;
        if (time >= RoundDura& !isRoundEnd)
        {
            EndRound();
        }
        if (time >= RoundAgain)
        {
            StartRound();
        }
    }
    private void EndRound()
    {
        isRoundEnd = true;
        Battle.Instance.ShowCharcter();
        BattleItem.Instance.NewRound();
        Debug.Log($"回合结束 - 时间: {Time.time:F1}");
    }
    private void StartRound()
    {
        time = 0f;
        isRoundEnd = false;
        OnRoundChanged?.Invoke();
        Battle.Instance.DisplayText = "";
        Debug.Log($"回合开始 - 时间: {Time.time:F1}");
    }
}