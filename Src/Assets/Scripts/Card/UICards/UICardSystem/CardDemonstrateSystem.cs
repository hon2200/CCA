using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��������ĵ���Ŀǰ����̫���������Ժ��ܲ��ܸĳ�ί�У�OnReadinMove�����Ǵ��ݲ�����һ�����⡣
//�����Ҿ�����������ϳ̶�Ҳ�ܸ�
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
    //����һ����������չʾ
    private void CreateDemonstratingCard(GameObject oriCard, int target)
    {
        //Copy����
        GameObject demoCard = Instantiate(oriCard, transform);
        //�ָ�ԭ״����С
        demoCard.GetComponent<CardSelection>().OnHoverExit();
        demoCard.transform.localScale = Vector3.one * 0.3f;
        demoCard.transform.rotation = new();
        //ɾȥCardSelection
        //��������ֵCardDemo
        var cardSelection = demoCard.GetComponent<CardSelection>();
        var cardDemo = demoCard.AddComponent<CardDemostration>();
        cardDemo.target = target;
        cardDemo.Glow = cardSelection.Glow;
        Destroy(cardSelection);
        //�ƶ���card��ͼ��
        demoCard.layer = LayerMask.NameToLayer("CardDemo");
        //����presentCard
        presentCard.Add(demoCard);
        ArrangePosition();
    }
    //�Ų���������չʾ�Ŀ���
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
