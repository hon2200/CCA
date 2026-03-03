using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control panel for debugging/adjusting player status (Sword, Bullet, HP) and killing the player.
/// Wire 7 buttons in code: [+Sword], [-Sword], [+Bullet], [-Bullet], [+HP], [-HP], [Kill].
/// If TargetPlayer is not set, uses the first human player from PlayerManager.
/// </summary>
public class PlayerStatusControl : MonoBehaviour
{
    [Tooltip("Leave empty to use first human player from PlayerManager.")]
    [SerializeField] private Player targetPlayer;

    [Tooltip("Optional: assign exactly 7 buttons in order [+Sword], [-Sword], [+Bullet], [-Bullet], [+HP], [-HP], [Kill]. If empty, will use first 7 Button components found in children.")]
    [SerializeField] private Button[] buttons = new Button[7];

    private const int IndexAddSword = 0;
    private const int IndexRemoveSword = 1;
    private const int IndexAddBullet = 2;
    private const int IndexRemoveBullet = 3;
    private const int IndexAddHP = 4;
    private const int IndexRemoveHP = 5;
    private const int IndexKill = 6;

    private void Start()
    {
        ResolveTargetPlayer();
        CacheButtons();
        BindAllButtons();
    }

    private void ResolveTargetPlayer()
    {
        if (targetPlayer != null) return;
        if (PlayerManager.Instance != null && PlayerManager.Instance.HumanPlayers.Count > 0)
            targetPlayer = PlayerManager.Instance.HumanPlayers[0];
    }

    private void CacheButtons()
    {
        if (buttons != null && buttons.Length == 7 && buttons[0] != null)
            return;
        var found = GetComponentsInChildren<Button>(true);
        if (found != null && found.Length >= 7)
        {
            buttons = new Button[7];
            for (int i = 0; i < 7; i++)
                buttons[i] = found[i];
        }
    }

    private void BindAllButtons()
    {
        if (buttons == null || buttons.Length < 7) return;
        if (buttons[IndexAddSword] != null) buttons[IndexAddSword].onClick.AddListener(AddSword);
        if (buttons[IndexRemoveSword] != null) buttons[IndexRemoveSword].onClick.AddListener(RemoveSword);
        if (buttons[IndexAddBullet] != null) buttons[IndexAddBullet].onClick.AddListener(AddBullet);
        if (buttons[IndexRemoveBullet] != null) buttons[IndexRemoveBullet].onClick.AddListener(RemoveBullet);
        if (buttons[IndexAddHP] != null) buttons[IndexAddHP].onClick.AddListener(AddHP);
        if (buttons[IndexRemoveHP] != null) buttons[IndexRemoveHP].onClick.AddListener(RemoveHP);
        if (buttons[IndexKill] != null) buttons[IndexKill].onClick.AddListener(KillPlayer);
    }

    private Player GetPlayer()
    {
        ResolveTargetPlayer();
        return targetPlayer;
    }

    // --- Public API for the 7 actions ---

    public void AddSword()
    {
        var p = GetPlayer();
        if (p == null) return;
        p.status.resources.Sword.Get(p, 1);
    }

    public void RemoveSword()
    {
        var p = GetPlayer();
        if (p == null) return;
        p.status.resources.Sword.Lost(1);
    }

    public void AddBullet()
    {
        var p = GetPlayer();
        if (p == null) return;
        p.status.resources.Bullet.Get(p, 1);
    }

    public void RemoveBullet()
    {
        var p = GetPlayer();
        if (p == null) return;
        p.status.resources.Bullet.Lost(1);
    }

    public void AddHP()
    {
        var p = GetPlayer();
        if (p == null) return;
        p.status.HP.Heal(1);
    }

    public void RemoveHP()
    {
        var p = GetPlayer();
        if (p == null) return;
        p.status.HP.Drain(1);
    }

    public void KillPlayer()
    {
        var p = GetPlayer();
        if (p == null) return;
        p.status.life.Dying();
    }
}
