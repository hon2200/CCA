using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AYellowpaper.SerializedCollections;

//���������Prefab���ϣ���ȡ������Լ���UI����ʼ��֮�����UITextManager����UIText����ͳһ����
//����PlayerStatue������߼���Ϣ��������UI����ʵ��Ϣ�Ĵ���
public class PlayerUIText : MonoBehaviour
{
    public Player player;
    //��ҵ�ͼ��
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
                    // ��HP�仯�¼�
                    //������õ�д��û��������������
                    player.status.HP.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, newVal, player.status.MaxHP);
                    // ��ʼ����
                    UpdatePlayerText(text.Value,
                        player.status.HP.Value,
                        player.status.MaxHP);
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
                        UpdatePlayerText(text.Value, player.status.resources.AvailableSword.Value, newVal);
                    // ��ʼ����
                    UpdatePlayerText(text.Value, player.status.resources.AvailableSword.Value,
                        player.status.resources.Sword.Value);
                    break;
                case PlayerUITextName.ID:
                    text.Value.text = player.ID_inGame.ToString();
                    break;
                case PlayerUITextName.AvailableAction:
                    string result = string.Join("\n", player.AvailableActions);
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
                    case PlayerUITextName.Honesty:
                        aiPlayer.Honest.OnValueChanged += (oldVal, newVal, opType) =>
                        UpdatePlayerText(text.Value, aiPlayer.Honest.Value);
                        UpdatePlayerText(text.Value, aiPlayer.Honest.Value);
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
        };
    }
    private void OntheEdgeofDeath()
    {
        spriteRenderer.color = Color.gray;
        Debug.Log("Player" + player.ID_inGame + "Have done it now...");
    }
    // ���������������ı���ʾ
    private void UpdatePlayerText(TextMeshPro textElement, object value, object maxValue = null)
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
    AvailableAction = 4,
    Character = 5,
    Honesty = 6,
    Emotion = 7,
    Intention = 8,
}