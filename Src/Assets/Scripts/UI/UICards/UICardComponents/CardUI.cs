using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class CardUI : MonoBehaviour
{
    //有些卡牌的文本内容并不适合关联，比如说激光炮
    public bool DoBind = true;
    //这个Dictionary里面的值可以是null不报错，这个兼容性还是要有的。
    [SerializeField]
    public SerializedDictionary<CardUITextName, TextMeshPro> UIText;

    //管理图片
    public SpriteRenderer sprite;
    public SpriteRenderer comsume_icon_sprite;

    //管理UI层级
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
                        //目前只考虑单一补给、单一消耗，未来如果有多的补给可以考虑用文字表示？
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

//目前只考虑单一补给、单一消耗，未来如果有多的补给可以考虑用文字表示？
public enum CardUITextName
{
    Name = 0,
    Consume = 1,
    Level = 3,
    Damage = 4,
    Discription = 5
}