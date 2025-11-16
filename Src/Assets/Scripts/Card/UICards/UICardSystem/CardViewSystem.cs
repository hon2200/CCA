using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CardViewSystem : MonoSingleton<CardViewSystem>
{
    public GameObject CardPanel;
    public List<GameObject> Cards;
    public Button AdvanceButton;
    public void Start()
    {
        AdvanceButton.onClick.AddListener(Clear);
        AdvanceButton.onClick.AddListener(() =>
        {
            PlayerSkillManager.Instance.OpenSkillPanel();
        });
    }
    public void Show(LevelDefine levelDefine)
    {
        CardPanel.SetActive(true);
        CreateAllCards(levelDefine);
        ArrangeCards();
    }
    private void Clear()
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
    private void CreateAllCards(LevelDefine levelDefine)
    {
        foreach(var cardID in levelDefine.UnlockedAction)
        {
            CardLiberary.Instance.CardDictionary.TryGetValue(cardID, out var card);
            GameObject newCard = CardPresentSystem.Instance.CreateCard(card, CardPanel.transform);
            //移动到card外图层
            newCard.layer = LayerMask.NameToLayer("CardView");
            SortingGroup sg = newCard.GetComponent<SortingGroup>();
            sg.sortingLayerName = "Pop up Panel";
            Cards.Add(newCard);
            // Promote Order
            CardUI cardUI = newCard.GetComponent<CardUI>();
            cardUI.PromoteLayer(2);
            //Smaller
            newCard.transform.localScale *= 0.6f;
        }
    }
    private void ArrangeCards()
    {
        CardArranger newCardArranger = new();
        newCardArranger.handCards = Cards;
        newCardArranger.ArrangeLine(5f);
    }
}
