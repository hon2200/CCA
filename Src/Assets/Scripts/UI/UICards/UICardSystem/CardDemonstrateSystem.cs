using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这个东西的调用目前还不太合理，看看以后能不能改成委托，OnReadinMove，但是传递参数是一个问题。
public class CardDemonstrateSystem : MonoSingleton<CardDemonstrateSystem>
{
    public List<GameObject> presentCard;
    //复制一个卡牌用于展示
    public void CreateDemonstratingCard(GameObject oriCard, int target)
    {
        //Copy卡牌
        GameObject demoCard = Instantiate(oriCard, transform);
        //恢复原状并缩小
        demoCard.GetComponent<CardSelection>().OnHoverExit();
        demoCard.transform.localScale = Vector3.one * 0.3f;
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
    //删除展示卡牌
    public void DeleteDemonstratingCard(GameObject oriCard)
    {
        oriCard.SetActive(false);
        presentCard.Remove(oriCard);
    }
    public void DeleteAllDemonstratingCard()
    {
        foreach(var card in presentCard)
        {
            card.SetActive(false);
        }
        presentCard.Clear();
    }
}
