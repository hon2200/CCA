using Common.Data;
using GameServer.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Character;

public class BattleItem:MonoSingleton<BattleItem>
{
    public delegate void OnCardChoose();
    public static event OnCardChoose Choosed;

    public GameObject bookItem;
    public TextMeshProUGUI Cost;
    public TextMeshProUGUI Hurt;
    public TextMeshProUGUI ATK;
    public GameObject ShowHurt;
    public GameObject ShowCost;
    public TextMeshProUGUI Name;
    public UIBookmessage message;

    public RectTransform This;
    public Image image;


    public pos CardRoot;


    private int id = -1;
    private int target = -1;
    public bool Choose = false;
    public pos enemyRoot;
    public pos UserRoot;
    public Character user;
    public List<Character> enemys = new List<Character>();
    public Dictionary<int,UIBookItem> uIBookItems = new Dictionary<int, UIBookItem>();
    public void Start()
    {
        UIBookItem.Choose += Select;
        Rule.OnRoundChanged += CardChanged;
        Character.TargetChoose += TargetChoose;
        BattleManager.Instance.BattleEnter();
        this.gameObject.SetActive(false);
        CardChanged();
    }
    public void CardChanged()
    {
        foreach (var kv in DataManager.Instance.Items)
        {
            bool can= CardFiltering.Instance.RoomChecks(kv.Value);
            bool cunzai = uIBookItems.ContainsKey(kv.Key);
            if (can && !cunzai)
            {
                GameObject go = Instantiate(bookItem, CardRoot.transform);
                UIBookItem ui = go.GetComponent<UIBookItem>();
                ui.SetItem(kv.Key, kv.Value);
                uIBookItems.Add(kv.Key, ui);
            }
            else if(!can && cunzai)
            {
                UIBookItem ui = uIBookItems[kv.Key];
                uIBookItems.Remove(kv.Key);
                Destroy(ui.gameObject);
            }
        }
    }
    public IEnumerator arrow()
    {
        while (true)
        {
            if (!Choose)
            {
                
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    This,
                    Input.mousePosition,
                    null,
                    out localPoint);
                float distance = localPoint.magnitude;
                image.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.x, distance);
                Vector2 centerPoint = (Vector2.zero + localPoint) / 2f;
                image.rectTransform.localPosition = centerPoint;
                float angle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;
                image.rectTransform.localRotation = Quaternion.Euler(0, 0, angle - 90);
                yield return new WaitForSeconds(0.03f);
            }
            else
            {
                break;
            }
            
        }
        
    }
    private void Select(int id)
    {
        this.gameObject.SetActive(true);
        uIBookItems.TryGetValue(id, out UIBookItem ui);
        if (ui.Item.ATK == 0)
        {
            ShowHurt.SetActive(false);
        }
        else
        {
            if (Hurt != null) Hurt = ui.Hurt;
            if (ATK != null) ATK = ui.ATK;
        }
        if (Cost != null)
        {
            Cost = ui.Cost;
            if (ui.Item.Cost <= 0) ShowCost.SetActive(false);
        }
        Name = ui.Name;
        this.gameObject.transform.position = ui.image.transform.position;
        if (ui.Item.TargetType == TargetType.自己 || ui.Item.TargetType == TargetType.敌方全体)
        {
            this.id = id;
            target = User.Instance.ID;
            List<int> s = new List<int>();
            s.Add(target);
            Battle.Instance.Add(User.Instance.ID, id, s);
            this.gameObject.SetActive(false);
            Debug.Log("你选择了" + ui.Item.Name);
            BattleCardGroup.Instance.UpdayeData(ui.Item.Icon, ui.Item.Name, ui.Item.Description);
            Choosed?.Invoke();
            Choose = true;
        }
        else
        {
            image.gameObject.SetActive(true);
            StartCoroutine(arrow());
            this.id = id;
        }
    }
    private void TargetChoose(int targetId)
    {
        if(Choose|| id ==-1) return;
        if (targetId == User.Instance.ID) return;
        image.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        this.target = targetId;
        List<int> s = new List<int>();
        s.Add(target);
        Battle.Instance.Add(User.Instance.ID, id, s);
        Debug.Log("你选择了卡牌" + id+"选择目标"+ targetId);
        uIBookItems.TryGetValue(id, out UIBookItem ui);
        BattleCardGroup.Instance.UpdayeData(ui.Item.Icon, ui.Item.Name, ui.Item.Description);
        Choosed?.Invoke();
        Choose = true;

    }
    public void NewRound()
    {
        Choose = false;
        target = -1;
        id = -1;
    }
    public void Set(Character cha)
    {
        if (Room.Instance.room.NumberID.Contains(cha.ID))
        {
            if (cha.ID == User.Instance.ID)
            {
                cha.transform.SetParent(UserRoot.transform, false); ;
                user = cha;
            }
            else
            {
                cha.transform.SetParent(enemyRoot.transform, false); ;
                enemys.Add(cha);
            }
        }
    }
    public void TypeSelect(int type)
    {

        ItemType tt = (ItemType)Enum.ToObject(typeof(ItemType), type);
        foreach (KeyValuePair<int, UIBookItem> pair in uIBookItems)
        {
            UIBookItem item = pair.Value;
            if (type == -2)
            {
                if (item.Item.ItemType == ItemType.近战 || item.Item.ItemType == ItemType.远程)
                {
                    item.gameObject.SetActive(true);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
            else
            {
                if (item.Item.ItemType != tt)
                {
                    item.gameObject.SetActive(false);
                }
                else
                {
                    item.gameObject.SetActive(true);
                }
            }

        }
    }
}
