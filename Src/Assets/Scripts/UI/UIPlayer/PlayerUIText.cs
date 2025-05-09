using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AYellowpaper.SerializedCollections;

//挂载在玩家Prefab身上，获取到玩家自己的UI，初始化之后读入UITextManager，在UIText里面统一管理
public class PlayerUIText : MonoBehaviour
{
    public Player player;
    [SerializeField]
    public SerializedDictionary<PlayerUITextName, TextMeshProUGUI> UIText;
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
                        UpdatePlayerText(text.Value, newVal, player.status.playerDefine.MaxHP);
                    // 初始更新
                    UpdatePlayerText(text.Value,
                        player.status.HP.Value,
                        player.status.playerDefine.MaxHP);
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
                        UpdatePlayerText(text.Value, player.status.resources.Sword.Value, newVal);
                    // 初始更新
                    UpdatePlayerText(text.Value, player.status.resources.AvailableSword.Value,
                        player.status.resources.Sword.Value);
                    break;
                case PlayerUITextName.ID:
                    text.Value.text = player.ID_inGame.ToString();
                    break;
            }
        }
        
    }
    // 辅助方法：更新文本显示
    private void UpdatePlayerText(TextMeshProUGUI textElement, object value, object maxValue = null)
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