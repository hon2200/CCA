using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CardViewSystem : MonoSingleton<CardViewSystem>
{
    public GameObject CardPanel;
    public List<GameObject> Cards;
    public void Show(LevelDefine levelDefine)
    {
        CardPanel.SetActive(true);
        CreateAllCards(levelDefine);
        ArrangeCards();
    }

    public void Clear()
    {
        // This is safe, because destroy won't destroy at once--from GPT
        foreach (var card in Cards)
        {
            if (card != null)
            {
                Destroy(card);
            }
        }
        Cards.Clear();
        CardPanel.SetActive(false);
    }

    public void CreateAllCards(LevelDefine levelDefine)
    {
        foreach(var cardID in levelDefine.UnlockedAction)
        {
            CardLiberary.Instance.CardDictionary.TryGetValue(cardID, out var card);
            GameObject newCard = CardPresentSystem.Instance.CreateCard(card, CardPanel.transform);
            //移动到card外图层
            newCard.layer = LayerMask.NameToLayer("CardView");
            Cards.Add(newCard);
        }
    }
    public void ArrangeCards()
    {
        CardArranger newCardArranger = new();
        newCardArranger.handCards = Cards;
        newCardArranger.ArrangeLine();
    }

}
