using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//?????????Prefab??????????????????UI?????????????UITextManager????UIText??????????
//????PlayerStatue???????????????????UI?????????????
public class PlayerUIText : MonoBehaviour
{
    public Player player;
    //Player Sprite
    public SpriteRenderer spriteRenderer;
    public List<SpriteRenderer> highlightRenderer;
    //攻击力和伤害加成
    public SpriteRenderer ATK;
    public SpriteRenderer DMG;
    //???????
    public GameObject Glow;
    [SerializeField]
    public SerializedDictionary<PlayerUITextName, TextMeshPro> UIText;
    public HPFilling HPFilling;
    public TMP_Text SkillText;
    public void Initialize()
    {
        InitializeImagesFromLibrary();
        UpdateSkillText();
        foreach(var text in UIText)
        {
            switch (text.Key)
            {
                case PlayerUITextName.HP:
                    // ??HP??????
                    //???????????????????????????
                    player.status.HP.OnValueChanged += (oldVal, newVal, opType) =>
                    {
                        UpdatePlayerText(text.Value, newVal, player.status.MaxHP);
                        SetHpBar(newVal, player.status.MaxHP);
                    };
                    // ???????
                    UpdatePlayerText(text.Value,
                        player.status.HP.Value,
                        player.status.MaxHP);
                    break;
                case PlayerUITextName.Bullet:
                    // ???????????
                    player.status.resources.Bullet.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, newVal);
                    // ???????
                    UpdatePlayerText(text.Value, player.status.resources.Bullet.Value);
                    break;
                case PlayerUITextName.Sword:
                    // ???????????????
                    player.status.resources.Sword.AvailableSword.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, newVal, player.status.resources.Sword.Value);
                    player.status.resources.Sword.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, player.status.resources.Sword.AvailableSword.Value, newVal);
                    // ???????
                    UpdatePlayerText(text.Value, player.status.resources.Sword.AvailableSword.Value,
                        player.status.resources.Sword.Value);
                    break;
                case PlayerUITextName.ID:
                    text.Value.text = player.ID_inGame.ToString();
                    break;
                case PlayerUITextName.Name:
                    text.Value.text = player.Name.ToString();
                    break;
                case PlayerUITextName.AvailableAction:
                    string result = string.Join("\n", player.hero.skills.Select(skill => skill.Name));
                    text.Value.text = result;
                    break;
                case PlayerUITextName.DMG:
                    UpdateDamagingOperatorDisplay();
                    break;
                case PlayerUITextName.ATK:
                    UpdateAttackingLevelOperatorDisplay();
                    break;
            }
        }

        player.status.buffs.OnListChanged += (list, message) =>
        {
            UpdateDamagingOperatorDisplay();
            UpdateAttackingLevelOperatorDisplay();
        };
        UpdateDamagingOperatorDisplay();
        UpdateAttackingLevelOperatorDisplay();

        player.status.life.OnValueChanged += (oldVal, newVal, opType) =>
        {
            if (opType == "Dying")
            {
                OntheEdgeofDeath();
            }
            if(opType == "Revive")
            {
                OnRivive();
            }
            if(opType == "Born")
            {
                OnBorn();
            }
        };

        SetSkillPanelVisible(false);
    }

    /// <summary>Sets SkillText from current hero skills (rich text). Called from <see cref="Initialize"/>; call again after <see cref="Player.SetHero"/>.</summary>
    public void UpdateSkillText()
    {
        if (SkillText == null) return;
        SkillText.richText = true;
        SkillText.text = BuildSkillsRichText(player?.hero?.skills);
    }

    /// <summary>Show or hide the skill description object. When showing, refreshes rich text from <see cref="UpdateSkillText"/>.</summary>
    public void SetSkillPanelVisible(bool visible)
    {
        if (SkillText == null) return;
        if (visible)
        {
            UpdateSkillText();
            SkillText.gameObject.SetActive(true);
        }
        else
            SkillText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Load character sprite from HeroLiberary or EnemyLiberary by player.hero.ID.
    /// Hero vs enemy: ID is looked up in HeroDataBase.HeroDictionary (hero) or HeroDataBase.EnemyDictionary (enemy); enemy type uses EnemyDefine.EnemyType.
    /// Uses template "Default" when ID has no template; for enemies, uses "BossDefault" when EnemyType is Boss.
    /// Sets spriteRenderer.sprite when the template is available.
    /// </summary>
    public void InitializeImagesFromLibrary()
    {
        if (player?.hero == null || spriteRenderer == null) return;
        string id = player.hero.ID;
        Sprite spriteToSet = null;

        if (HeroDataBase.Instance?.HeroDictionary != null && HeroDataBase.Instance.HeroDictionary.TryGetValue(id, out _))
        {
            var lib = HeroLiberary.Instance?.HeroDictionary;
            if (lib != null && (lib.TryGetValue(id, out var t) || lib.TryGetValue("Default", out t)))
                spriteToSet = t?.image;
        }
        else if (HeroDataBase.Instance?.EnemyDictionary != null && HeroDataBase.Instance.EnemyDictionary.TryGetValue(id, out var enemyDefine))
        {
            var lib = EnemyLiberary.Instance?.EnemyDictionary;
            if (lib != null)
            {
                if (lib.TryGetValue(id, out var t))
                    spriteToSet = t?.image;
                if (spriteToSet == null && string.Equals(enemyDefine.EnemyType, "Boss", System.StringComparison.OrdinalIgnoreCase) && lib.TryGetValue("BossDefault", out var bossT))
                    spriteToSet = bossT?.image;
                if (spriteToSet == null && lib.TryGetValue("Default", out var defaultT))
                    spriteToSet = defaultT?.image;
            }
        }

        if (spriteToSet != null)
        {
            spriteRenderer.sprite = spriteToSet;
            foreach(var renderer in highlightRenderer)
            {
                renderer.sprite = spriteToSet;
            }
        }
            
    }
    private void OntheEdgeofDeath()
    {
        spriteRenderer.color = Color.gray;
        Debug.Log("Player" + player.ID_inGame + "Have done it now...");
    }
    private void OnRivive()
    {
        spriteRenderer.color = Color.red;
        Debug.Log("Player" + player.ID_inGame + "Revives!");
    }
    private void OnBorn()
    {
        Debug.Log("Player" + player.ID_inGame + "is Born");
        StartCoroutine(ShineEffect());
    }
    private IEnumerator ShineEffect()
    {
        SpriteRenderer glowRenderer = Glow.GetComponent<SpriteRenderer>();
        Color baseColor = glowRenderer.color;

        float duration = 1f;
        float elapsed = 0f;
        Glow.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.PingPong(t * 2f, 1f); // fade in/out
            glowRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        glowRenderer.color = baseColor;
        Glow.SetActive(false);
    }

    public void SetHpBar(float currentHp, float maxHp)
    {
        float fill = currentHp / (float)(maxHp);
        HPFilling.MovingMask(fill);
    }

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

        var finalSb = sb.ToString();
        Debug.Log(finalSb);
        return finalSb;
    }

    private static string TmpEscapeForRichText(string s)
    {
        if (string.IsNullOrEmpty(s))
            return "";
        return s.Replace("\\", "\\\\").Replace("<", "\\<");
    }

    // ????????????????????
    private void UpdatePlayerText(TextMeshPro textElement, object value, object maxValue = null)
    {
        if (textElement == null) return;

        textElement.text = maxValue != null
            ? $"{value}/{maxValue}"
            : value.ToString();
    }

    /// <summary>
    /// If player's status.buffs has no DamagingOperator or effective value is zero, set DMG sprite inactive.
    /// Otherwise set active and display the damaging operator in the DMG text (e.g. "*3", "+1*2", "+2*3+1").
    /// </summary>
    private void UpdateDamagingOperatorDisplay()
    {
        if (DMG == null) return;
        var damagingOps = player?.status?.buffs?.OfType<DamagingOperator>().ToList() ?? new List<DamagingOperator>();
        bool hasNonZero = damagingOps.Count > 0;
        DMG.gameObject.SetActive(hasNonZero);
        if (UIText != null && UIText.TryGetValue(PlayerUITextName.DMG, out var dmgText) && dmgText != null)
            dmgText.text = hasNonZero ? GetOperatorDisplayString(damagingOps.Cast<BuffOperator>().ToList()) : "";
    }

    /// <summary>
    /// If player's status.buffs has no AttackingLevelOperator or effective value is zero, set ATK sprite inactive.
    /// Otherwise set active and display the attack operator in the ATK text (e.g. "*3", "+1*2", "+2*3+1").
    /// </summary>
    private void UpdateAttackingLevelOperatorDisplay()
    {
        if (ATK == null) return;
        var attackOps = player?.status?.buffs?.OfType<AttackingLevelOperator>().ToList() ?? new List<AttackingLevelOperator>();
        bool hasNonZero = attackOps.Count > 0;
        ATK.gameObject.SetActive(hasNonZero);
        if (UIText != null && UIText.TryGetValue(PlayerUITextName.ATK, out var atkText) && atkText != null)
            atkText.text = hasNonZero ? GetOperatorDisplayString(attackOps.Cast<BuffOperator>().ToList()) : "";
    }

    /// <summary>Builds a single display string from one or more BuffOperators (e.g. "*3", "+1*2", "+2*3+1").</summary>
    private static string GetOperatorDisplayString(List<BuffOperator> ops)
    {
        if (ops == null || ops.Count == 0) return "";
        if (ops.Count == 1) return ops[0].GetDisplayString();
        // Multiple: join each buff's display with a space
        return string.Join(" ", ops.Select(o => o.GetDisplayString()).Where(s => !string.IsNullOrEmpty(s)));
    }
}
//????????binding name??????????UI Text???????????????????
[System.Serializable]
public class UITextBindingConfig
{
    public PlayerUITextName Name;
    public TextMeshProUGUI targetText;
    public UITextBindingConfig(PlayerUITextName Name, TextMeshProUGUI targetText)
    {
        this.Name = Name;
        this.targetText = targetText;
    }
}
public enum PlayerUITextName
{
    ID = 0,
    Name = 1,
    HP = 2,
    Bullet = 3,
    Sword = 4,
    AvailableAction = 5,
    ATK = 6,
    DMG = 7,
}