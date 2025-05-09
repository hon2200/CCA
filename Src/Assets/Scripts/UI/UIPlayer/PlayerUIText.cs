using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AYellowpaper.SerializedCollections;

//���������Prefab���ϣ���ȡ������Լ���UI����ʼ��֮�����UITextManager����UIText����ͳһ����
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
                    // ��HP�仯�¼�
                    //������õ�д��û��������������
                    player.status.HP.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, newVal, player.status.playerDefine.MaxHP);
                    // ��ʼ����
                    UpdatePlayerText(text.Value,
                        player.status.HP.Value,
                        player.status.playerDefine.MaxHP);
                    break;
                case PlayerUITextName.Bullet:
                    // ���ӵ��仯�¼�
                    player.status.resources.Bullet.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, newVal);
                    // ��ʼ����
                    UpdatePlayerText(text.Value, player.status.resources.Bullet.Value);
                    break;
                case PlayerUITextName.Sword:
                    // �󶨽��Ϳ��ý��仯�¼�
                    player.status.resources.AvailableSword.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, newVal, player.status.resources.Sword.Value);
                    player.status.resources.Sword.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, player.status.resources.Sword.Value, newVal);
                    // ��ʼ����
                    UpdatePlayerText(text.Value, player.status.resources.AvailableSword.Value,
                        player.status.resources.Sword.Value);
                    break;
                case PlayerUITextName.ID:
                    text.Value.text = player.ID_inGame.ToString();
                    break;
            }
        }
        
    }
    // ���������������ı���ʾ
    private void UpdatePlayerText(TextMeshProUGUI textElement, object value, object maxValue = null)
    {
        if (textElement == null) return;

        textElement.text = maxValue != null
            ? $"{value}/{maxValue}"
            : value.ToString();
    }
}
//ͨ����ͬ��binding name������ҵĶ��UI Text������Ѫ�����ӵ����ȵ�
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