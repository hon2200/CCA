using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class CardUI : MonoBehaviour
{
    //��Щ���Ƶ��ı����ݲ����ʺϹ���������˵������
    public bool DoBind = true;
    //���Dictionary�����ֵ������null��������������Ի���Ҫ�еġ�
    [SerializeField]
    public SerializedDictionary<CardUITextName, TextMeshPro> UIText;

    //����ͼƬ
    public SpriteRenderer sprite;
    public SpriteRenderer comsume_icon_sprite;

    //����UI�㼶
    public Canvas cardCanvas;
    public SortingGroup sortingGroup;
    public void Initialize(CardTemplete cardTemplete)
    {
        if (!DoBind)
            return;
        ActionDataBase.Instance.ActionDictionary.TryGetValue(cardTemplete.ID, out var actionDefine);
        if (actionDefine != null)
            foreach (var text in UIText)
            {
                if (text.Value == null)
                    continue;
                switch (text.Key)
                {
                    case CardUITextName.Consume:
                        //Ŀǰֻ���ǵ�һ��������һ���ģ�δ������ж�Ĳ������Կ��������ֱ�ʾ��
                        foreach (var cost in actionDefine.Costs)
                        {
                            if (cost != 0)
                            {
                                text.Value.text = cost.ToString();
                            }
                        }
                        break;
                    case CardUITextName.Level:
                        if (actionDefine is AttackDefine attackDefine)
                        {
                            text.Value.text = attackDefine.Level.ToString();
                        }
                        break;
                    case CardUITextName.Damage:
                        if (actionDefine is AttackDefine attackDefine_)
                        {
                            text.Value.text = attackDefine_.Damage.ToString();
                        }
                        break;
                    case CardUITextName.Discription:
                        text.Value.text = actionDefine.Description;
                        break;
                    case CardUITextName.Name:
                        text.Value.text = actionDefine.Name;
                        break;
                }
            }
        else
            Debug.Assert(false, "Can't Find ActionDefine");
        InitializeSprite(cardTemplete);
    }
    private void InitializeSprite(CardTemplete cardTemplete)
    {
        sprite.sprite = cardTemplete.image;
        comsume_icon_sprite.sprite = cardTemplete.comsume_image;
    }
    public void PromoteLayer(int n)
    {
        cardCanvas.sortingOrder += n;
        sortingGroup.sortingOrder += n;
    }
    public void PromoteLayerTo(int n)
    {
        cardCanvas.sortingOrder = n;
        sortingGroup.sortingOrder = n;
    }
}

//Ŀǰֻ���ǵ�һ��������һ���ģ�δ������ж�Ĳ������Կ��������ֱ�ʾ��
public enum CardUITextName
{
    Name = 0,
    Consume = 1,
    Level = 3,
    Damage = 4,
    Discription = 5
}