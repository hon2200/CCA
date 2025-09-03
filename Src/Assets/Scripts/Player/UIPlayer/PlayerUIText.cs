using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AYellowpaper.SerializedCollections;

//挂载在玩家Prefab身上，获取到玩家自己的UI，初始化之后读入UITextManager，在UIText里面统一管理
//处理PlayerStatue里面的逻辑信息到界面上UI的真实信息的传递
public class PlayerUIText : MonoBehaviour
{
    public Player player;
    //玩家的图像
    public SpriteRenderer spriteRenderer;
    [SerializeField]
    public SerializedDictionary<PlayerUITextName, TextMeshPro> UIText;
    public void Initialize()
    {
        foreach(var text in UIText)
        {
            switch (text.Key)
            {
                case PlayerUITextName.HP:
                    // 绑定HP变化事件
                    //这里采用的写法没有声明函数主题
                    player.status.HP.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, newVal, player.status.MaxHP);
                    // 初始更新
                    UpdatePlayerText(text.Value,
                        player.status.HP.Value,
                        player.status.MaxHP);
                    break;
                case PlayerUITextName.Bullet:
                    // 绑定子弹变化事件
                    player.status.resources.Bullet.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, newVal);
                    // 初始更新
                    UpdatePlayerText(text.Value, player.status.resources.Bullet.Value);
                    break;
                case PlayerUITextName.Sword:
                    // 绑定剑和可用剑变化事件
                    player.status.resources.AvailableSword.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, newVal, player.status.resources.Sword.Value);
                    player.status.resources.Sword.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, player.status.resources.AvailableSword.Value, newVal);
                    // 初始更新
                    UpdatePlayerText(text.Value, player.status.resources.AvailableSword.Value,
                        player.status.resources.Sword.Value);
                    break;
                case PlayerUITextName.ID:
                    text.Value.text = player.ID_inGame.ToString();
                    break;
            }
        }

        player.status.life.OnValueChanged += (oldVal, newVal, opType) =>
        {
            if (opType == "Die")
            {
                OntheEdgeofDeath();
            }
        };
    }
    private void OntheEdgeofDeath()
    {
        spriteRenderer.color = Color.gray;
        Debug.Log("Player" + player.ID_inGame + "Have done it now...");
    }
    // 辅助方法：更新文本显示
    private void UpdatePlayerText(TextMeshPro textElement, object value, object maxValue = null)
    {
        if (textElement == null) return;

        textElement.text = maxValue != null
            ? $"{value}/{maxValue}"
            : value.ToString();
    }
}
//通过不同的binding name管理玩家的多个UI Text，比如血量、子弹数等等
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
    HP = 1,
    Bullet = 2,
    Sword = 3,
}