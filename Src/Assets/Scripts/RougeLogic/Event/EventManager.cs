using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;


public class EventManager : MonoSingleton<EventManager>
{
    private const string DefaultEventBackgroundKey = "Default";

    [SerializeField] private GameObject EventPanel; //最大的事件面板
    [SerializeField] private TMP_Text EventTitle;
    [SerializeField] private TMP_Text EventDescription;
    [SerializeField] private GameObject ButtonPrefab;
    [SerializeField] private Transform EventButtonPanel;
    [SerializeField] private Button AdvanceButton;
    [SerializeField] private SerializedDictionary<string, Sprite> EventBackGroundDic;
    [SerializeField] private SpriteRenderer EventBackground;
    public Transform EventBackgroundTransform => EventBackground != null ? EventBackground.transform : null;

    [Header("Curse Fusion (optional)")]
    [SerializeField] private Button curseFusionConfirmButton;
    private CurseFusion currentCurseFusion;
    private int curseFusionStep;


    public void Awake()
    {
        AdvanceButton.onClick.AddListener(Advance);
    }

    //private void Start()
    //{
    //    InitChooseRelic();
    //}
    public void Init(EventDefine eventDefine)
    {
        ClearOptionButtons();
        EventTitle.text = eventDefine.Name;
        EventDescription.text = eventDefine.Description;
        ApplyEventBackground(eventDefine.ID);
        foreach (var option in eventDefine.Options)
        {
            var newOption = Instantiate(ButtonPrefab, EventButtonPanel);
            var eventUI = newOption.GetComponent<EventOptionButtonUI>();
            eventUI.UpdateTitle(option);
            string capturedOption = option;
            eventUI.AddListener(() =>
            {
                eventDefine.OnChoose(capturedOption);
                CompletedSelection();
            });
        }
    }

    private void ApplyEventBackground(string eventId)
    {
        if (EventBackground == null)
            return;

        Sprite sprite = null;
        if (EventBackGroundDic != null)
        {
            if (EventBackGroundDic.TryGetValue(eventId, out var byId) && byId != null)
                sprite = byId;
            if (sprite == null && EventBackGroundDic.TryGetValue(DefaultEventBackgroundKey, out var fallback) && fallback != null)
                sprite = fallback;
        }

        if (sprite != null)
            EventBackground.sprite = sprite;

        EventBackground.gameObject.SetActive(true);
    }
    public void InitRecruitHero()
    {
        var eventDefine = new RecruitHero();
        Init(eventDefine);
    }
    public void InitChooseRelic()
    {
        var eventDefine = new ChooseRelic();
        Init(eventDefine);
    }
    public void InitChooseCard()
    {
        var eventDefine = new ChooseCard();
        Init(eventDefine);
    }
    public void InitTreasureEvent()
    {
        var eventDefine = new TreasureEvent();
        Init(eventDefine);
    }

    public void InitCurseFusionEvent()
    {
        currentCurseFusion = new CurseFusion();
        Init(currentCurseFusion);
        curseFusionStep = 0;

        RougePlayerUI.Instance?.SetHeroPanelActive(true);
        RougePlayerUI.Instance?.BuildRougeHeroDropDown();

        if (curseFusionConfirmButton == null)
        {
            Debug.LogWarning("InitCurseFusion: curseFusionConfirmButton is not assigned.");
            return;
        }

        curseFusionConfirmButton.gameObject.SetActive(true);
        curseFusionConfirmButton.onClick.RemoveAllListeners();
        curseFusionConfirmButton.onClick.AddListener(OnCurseFusionConfirmClicked);
        AdvanceButton.gameObject.SetActive(false);
    }

    private void OnCurseFusionConfirmClicked()
    {
        if (currentCurseFusion == null)
            return;

        Hero selectedHero = RougePlayerUI.Instance?.GetSelectedRougeHero();
        if (selectedHero == null)
        {
            Debug.LogWarning("CurseFusion: no hero selected.");
            return;
        }

        if (curseFusionStep == 0)
        {
            currentCurseFusion.hero1 = selectedHero;
            curseFusionStep = 1;
            RougePlayerUI.Instance?.BuildRougeHeroDropDown();
            return;
        }

        if (selectedHero == currentCurseFusion.hero1)
        {
            Debug.LogWarning("CurseFusion: please select a different second hero.");
            return;
        }

        currentCurseFusion.hero2 = selectedHero;
        currentCurseFusion.MergeHero();

        RougePlayerUI.Instance?.BuildRougeHeroDropDown();
        RougePlayerUI.Instance?.SetHeroPanelActive(false);

        curseFusionConfirmButton.gameObject.SetActive(false);
        currentCurseFusion = null;
        CompletedSelection();
    }

    public void SetEvent()
    {
        EventPanel.SetActive(true);
        EventButtonPanel.gameObject.SetActive(true);
        AdvanceButton.gameObject.SetActive(false);
    }
    private void CompletedSelection()
    {
        EventButtonPanel.gameObject.SetActive(false);
        AdvanceButton.gameObject.SetActive(true);
    }
    private void Advance()
    {
        if (EventBackground != null)
            EventBackground.gameObject.SetActive(false);

        EventPanel.SetActive(false);
    }

    private void ClearOptionButtons()
    {
        if (EventButtonPanel == null)
            return;

        for (int i = EventButtonPanel.childCount - 1; i >= 0; i--)
            Destroy(EventButtonPanel.GetChild(i).gameObject);
    }
}
