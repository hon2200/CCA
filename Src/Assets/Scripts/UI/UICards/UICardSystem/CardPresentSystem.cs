using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.UI;

//UI互相遮挡关系管理说明：

/*
 * 不要通过z方向管理，通过Canvas的Sorting Order（对UI元素生效）和
 * 挂在Card上面的SortingGroup（对Sprite元素生效）进行管理
 * 卡牌之间的Order差为10，卡牌内部Order以1的step跳跃
 */

public class CardPresentSystem : MonoSingleton<CardPresentSystem>
{
    [SerializeField]
    public SerializedDictionary<ActionType, GameObject> CardMenu;
    [SerializeField]
    public SerializedDictionary<ActionType, Button> MenuButtons;
    [SerializeField]
    public  SerializedDictionary<ActionType, List<GameObject>> Cards_inType;

    //依据CardLiberary，按照类型创建卡牌
    public void CreateCards()
    {
        //先对每个类别卡牌清零
        foreach(var cards in Cards_inType)
        {
            foreach(var card in cards.Value)
            {
                Destroy(card);
            }
            cards.Value.Clear();
        }
        //开始创建卡牌
        foreach(var card in CardLiberary.Instance.CardDictionary)
        {
            //获得卡牌的actiondata
            ActionDataBase.Instance.ActionDictionary.TryGetValue(card.Key, out var actionData);
            if (actionData == null)
                Debug.Assert(false, "Wrong Key" + card.Key);
            //获得卡牌的类型所在的父物体
            CardMenu.TryGetValue(card.Value.actionType, out var menu);
            if (menu == null)
                Debug.Assert(false, "Can't fine Menu");
            var newCard = CreateCard(card.Value, menu.transform);
            //加入分类管理的卡牌资源
            Cards_inType.TryGetValue(card.Value.actionType, out var cards);
            cards.Add(newCard);
        }
    }
    //组织卡牌按扇形打开，具体实现下放到CardArranger类中
    public void ArrangeCards()
    {
        foreach(var cards in Cards_inType)
        {
            CardArranger newCardArranger = new();
            newCardArranger.handCards = cards.Value;
            newCardArranger.ArrangeCards();
        }

    }
    //处理每一个MenuButton的Onclick
    public void MenuButtonsReady()
    {
        foreach(var button in MenuButtons)
        {
            button.Value.onClick.AddListener(() =>
            OpenPenal(button.Key));               
        }
    }

    public void Start()
    {
        CreateCards();
        ArrangeCards();
        MenuButtonsReady();
        OpenPenal(ActionType.Supply);
    }
    #region 具体实现私有函数
    private GameObject CreateCard(CardTemplete cardTemplete, Transform parent)
    {
        var newCard = Instantiate(cardTemplete.prefab, parent);
        newCard.name = cardTemplete.name;
        //赋值Card组件
        var card = newCard.GetComponent<RunTimeCard>();
        ActionDataBase.Instance.ActionDictionary.TryGetValue(cardTemplete.ID, out var actionDefine);
        card.actionDefine = actionDefine;
        card.cardTemplete = cardTemplete;
        //赋值CardUI组件
        var cardUIText = newCard.GetComponent<CardUI>();
        cardUIText.Initialize(cardTemplete);
        return newCard;
    }
    //打开某一个卡牌菜单。
    private void OpenPenal(ActionType actionType)
    {
        foreach (var menu in CardMenu)
        {
            if (menu.Key != actionType)
                menu.Value.SetActive(false);
            else
                menu.Value.SetActive(true);
        }
    }

    #endregion
}

//按次序扇形打开卡牌
public class CardArranger
{
    public List<GameObject> handCards = new();
    private const float CenterRadius = 16.0f;
    private Vector3 centerPoint = new Vector3(0.0f, -19.8f, 0.0f);
    public void ArrangeCards()
    {
        ArrangePosition();
    }
    private void ArrangePosition()
    {
        const float angle = 5.0f;
        var cardAngle = (handCards.Count - 1) * angle / 2;
        int order = 0;
        for (var i = 0; i < handCards.Count; ++i)
        {
            // Rotate.
            var rotation = Quaternion.Euler(0, 0, cardAngle - i * angle);
            // Move.
            //高度值决定了卡牌的层级，但是UI的显示依然可能出问题
            order += 10;
            var position = CalculateCardPosition(cardAngle - i * angle);
            //Give Value.
            handCards[i].transform.position = position;
            handCards[i].transform.rotation = rotation;
            //PromoteLayer后面包括了对position的移动，所以要在这个后面执行
            handCards[i].GetComponent<CardUI>().PromoteLayerTo(order);
        }
    }
    private Vector3 CalculateCardPosition(float angle)
    {
        return new Vector3(
            centerPoint.x - CenterRadius * Mathf.Sin(Mathf.Deg2Rad * angle),
            centerPoint.y + CenterRadius * Mathf.Cos(Mathf.Deg2Rad * angle),
            0.0f);
    }
}
