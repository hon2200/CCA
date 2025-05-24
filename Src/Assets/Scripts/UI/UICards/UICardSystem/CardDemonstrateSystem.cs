using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��������ĵ���Ŀǰ����̫���������Ժ��ܲ��ܸĳ�ί�У�OnReadinMove�����Ǵ��ݲ�����һ�����⡣
public class CardDemonstrateSystem : MonoSingleton<CardDemonstrateSystem>
{
    public List<GameObject> presentCard;
    //����һ����������չʾ
    public void CreateDemonstratingCard(GameObject oriCard, int target)
    {
        //Copy����
        GameObject demoCard = Instantiate(oriCard, transform);
        //�ָ�ԭ״����С
        demoCard.GetComponent<CardSelection>().OnHoverExit();
        demoCard.transform.localScale = Vector3.one * 0.3f;
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
    //ɾ��չʾ����
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
