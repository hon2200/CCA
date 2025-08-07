using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这个东西的调用目前还不太合理，看看以后能不能改成委托，OnReadinMove，但是传递参数是一个问题。
//另外我觉得这个类的耦合程度也很高
public class CardDemonstrateSystem : MonoSingleton<CardDemonstrateSystem>
{
    public List<GameObject> presentCard;
    public void AddListener(Player player1)
    {
        player1.action.OnListChanged += (newList, message) =>
        {
            if (message == "Clear_End")
                DeleteAllDemonstratingCard();
            if (message == "Remove_Player" || message == "Add_Player")
                DemonstrateCards(newList);
        };
    }
    public void DemonstrateCards(List<ActionDefine> actionList)
    {
        DeleteAllDemonstratingCard();
        foreach(var action in actionList)
        {
            CardPresentSystem.Instance.FindCard(action.ID, out var card);
            if (card != null)
                CreateDemonstratingCard(card, action.Target);
        }
        ArrangePosition();
    }
    //复制一个卡牌用于展示
    private void CreateDemonstratingCard(GameObject oriCard, int target)
    {
        //Copy卡牌
        GameObject demoCard = Instantiate(oriCard, transform);
        //恢复原状并缩小
        demoCard.GetComponent<CardSelection>().OnHoverExit();
        demoCard.transform.localScale = Vector3.one * 0.3f;
        demoCard.transform.rotation = new();
        //删去CardSelection
        //创建并赋值CardDemo
        var cardSelection = demoCard.GetComponent<CardSelection>();
        var cardDemo = demoCard.AddComponent<CardDemostration>();
        cardDemo.target = target;
        cardDemo.Glow = cardSelection.Glow;
        Destroy(cardSelection);
        //移动到card外图层
        demoCard.layer = LayerMask.NameToLayer("CardDemo");
        //加入presentCard
        presentCard.Add(demoCard);
        ArrangePosition();
    }
    //排布所有用于展示的卡牌
    private void ArrangePosition()
    {
        for (var i = 0; i < presentCard.Count; ++i)
        {
            // Move.
            var position = new Vector3(0, -i, 0);
            //Give Value.
            presentCard[i].transform.localPosition = position;
        }
    }
    public void DeleteAllDemonstratingCard()
    {
        foreach(var card in presentCard)
        {
            Destroy(card);
        }
        presentCard.Clear();
        Arrow.Instance.DeActive();
    }
}
