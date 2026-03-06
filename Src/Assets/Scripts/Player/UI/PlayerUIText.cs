using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AYellowpaper.SerializedCollections;

//?????????Prefab??????????????????UI?????????????UITextManager????UIText??????????
//????PlayerStatue???????????????????UI?????????????
public class PlayerUIText : MonoBehaviour
{
    public Player player;
    //???????
    public SpriteRenderer spriteRenderer;
    //???????
    public GameObject Glow;
    [SerializeField]
    public SerializedDictionary<PlayerUITextName, TextMeshPro> UIText;
    public HPFilling HPFilling;
    public void Initialize()
    {
        InitializeImagesFromLibrary();
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
            }
            if (player is AIPlayer aiPlayer)
            {
                switch (text.Key)
                {
                    case PlayerUITextName.Character:
                        text.Value.text = aiPlayer.CharacterDefine.Name;
                        break;
                    case PlayerUITextName.Intention:
                        aiPlayer.IntendedType.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, aiPlayer.IntendedType.Value);
                        UpdatePlayerText(text.Value, aiPlayer.IntendedType.Value);
                        break;
                    case PlayerUITextName.Emotion:
                        aiPlayer.Emo.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, aiPlayer.Emo.emotionType);
                        UpdatePlayerText(text.Value, aiPlayer.Emo.emotionType);
                        break;
                }
            }
        }

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
            spriteRenderer.sprite = spriteToSet;
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

    // ????????????????????
    private void UpdatePlayerText(TextMeshPro textElement, object value, object maxValue = null)
    {
        if (textElement == null) return;

        textElement.text = maxValue != null
            ? $"{value}/{maxValue}"
            : value.ToString();
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
    Character = 6,
    Emotion = 7,
    Intention = 8
}