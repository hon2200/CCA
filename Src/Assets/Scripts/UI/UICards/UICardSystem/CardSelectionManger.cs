using UnityEngine;

//射线检测的状态
public enum RayStatus
{
    Disable = 0,//选择系统没有开启
    ChooseCard = 1,//在选择卡牌时
    OnHoverCard = 2,//在卡牌上停留
    DragCard = 3,//拖拽卡牌
    ChosseFirstTarget = 4,//在选择第一个目标时
    ChooseMultiTarget = 5,//在选择多个目标，当然同时也可以选择卡牌
}


//注：考虑为ReadinMove，或者Player的Onselect添加委托，但目前可能没有必要
//鼠标点击事件真不能用委托：要考虑不同RayStatus的情况，委托造成的分散式管理难度太大、太乱了。
//OnHoverEnter不用委托，原因也是因为其具体实现在CardSelection里面就处理好了
//架构替代方案：在这里面定义OnHoverEnter委托，然后传递GamObejct参数？真不行，不同参数（玩家还是卡牌差太多了），调用很麻烦，也不能写IHoverable接口了
//考虑为卡牌添加Tag，加一层射线判定，但目前没有必要，因为整个Card Layer就它一个人。Player同理。
public class CardSelectionManager : MonoSingleton<CardSelectionManager>
{
    //射线状态
    public RayStatus rayStatus;
    //保存上次停留的物件
    private GameObject lastHoveredCard;
    private GameObject lastHoveredPlayer;
    private GameObject lastHoveredCardDemo;
    public Player player1;
    public void Update()
    {
        MouseAndRayUtil.Hit("Card", out var card);
        //这个代码不好，之后处理isAvailable的位置把这个一起处理了。我感觉可以写在接口里面？
        //但是看看Card的逻辑上isAvailable和交互上的要不要分开？
        if (card != null)
            if (card.GetComponent<RunTimeCard>().isAvailable == false)
                card = null;
        MouseAndRayUtil.Hit("Player", out var player);
        MouseAndRayUtil.Hit("CardEffectiveArea", out var area);
        MouseAndRayUtil.Hit("CardDemo", out var cardDemo);
        switch (rayStatus)
        {
            case RayStatus.Disable:
                break;
            case RayStatus.ChooseCard:
                MouseAndRayUtil.RenewHitting(ref lastHoveredCard, card);
                MouseAndRayUtil.RenewHitting(ref lastHoveredCardDemo, cardDemo);
                //按下鼠标左键删除行动
                if(Input.GetMouseButtonDown(1))
                {
                    //这里两者的同步非常重要
                    if (cardDemo != null)
                    {
                        int order = CardDemonstrateSystem.Instance.presentCard.IndexOf(cardDemo);
                        player1.action.DeleteMoveAt(order, "Player");
                    }

                }
                //停留在卡牌上进入停留阶段
                if (card != null)
                {
                    rayStatus = RayStatus.OnHoverCard;
                    //退出展示
                    if (cardDemo != null)
                        cardDemo.GetComponent<IHoverable>().OnHoverExit();
                }
                break;
            case RayStatus.OnHoverCard:
                MouseAndRayUtil.RenewHitting(ref lastHoveredCard, card);
                //移走鼠标回到选牌阶段
                if (card == null)
                {
                    rayStatus = RayStatus.ChooseCard;
                }
                //鼠标在上面按住开始进入使用卡牌的下一阶段
                else if (Input.GetMouseButton(0))
                {
                    if (card.GetComponent<CardSelection>().HaveTarget())
                        rayStatus = RayStatus.ChosseFirstTarget;
                    else
                        rayStatus = RayStatus.DragCard;
                }
                break;
            case RayStatus.DragCard:
                //按住拖动卡牌
                if (Input.GetMouseButton(0))
                {
                    MouseAndRayUtil.FollowMouse(lastHoveredCard.transform);
                }
                //鼠标松开读入或放弃
                else if(Input.GetMouseButtonUp(0))
                {
                    if (area != null)
                    {
                        player1.action.ReadinMove(lastHoveredCard.GetComponent<RunTimeCard>().actionDefine.ID, player1.ID_inGame, "Player");
                        lastHoveredCard.GetComponent<CardSelection>().OnHoverExit();
                        rayStatus = RayStatus.ChooseCard;
                    }
                    else
                    {
                        lastHoveredCard.GetComponent<CardSelection>().OnHoverExit();
                        rayStatus = RayStatus.ChooseCard;
                    }
                }
                break;
            case RayStatus.ChosseFirstTarget:
                MouseAndRayUtil.RenewHitting(ref lastHoveredPlayer, player);
                //按住拖动箭头
                if (Input.GetMouseButton(0))
                {
                    Arrow.Instance.FromOriToMouse(lastHoveredCard.transform);
                }
                //鼠标松开读入或放弃
                else if(Input.GetMouseButtonUp(0))
                {
                    if (player != null)
                    {
                        int target = player.GetComponent<Player>().ID_inGame;
                        player1.action.ReadinMove(lastHoveredCard.GetComponent<RunTimeCard>().actionDefine.ID,
                            target, "Player");
                        lastHoveredCard.GetComponent<CardSelection>().OnHoverExit();
                        Arrow.Instance.DeActive();
                        rayStatus = RayStatus.ChooseMultiTarget;
                        player.GetComponent<PlayerSelection>().OnSelect();
                    }
                    else
                    {
                        lastHoveredCard.GetComponent<CardSelection>().OnHoverExit();
                        Arrow.Instance.DeActive();
                        rayStatus = RayStatus.ChooseCard;
                    }
                }
                break;
            case RayStatus.ChooseMultiTarget:
                MouseAndRayUtil.RenewHitting(ref lastHoveredPlayer, player);
                //鼠标按下读入
                if (Input.GetMouseButtonDown(0))
                {
                    if (player != null)
                    {
                        int target = player.GetComponent<Player>().ID_inGame;
                        player1.action.ReadinMove(lastHoveredCard.GetComponent<RunTimeCard>().actionDefine.ID,
                            target, "Player");
                        Debug.Log("ReadinMove" + lastHoveredCard.GetComponent<RunTimeCard>().actionDefine.ID);
                        player.GetComponent<PlayerSelection>().OnSelect();
                    }
                }
                //如果选中其他卡牌，则进入选牌状态
                if (card != null)
                {
                    UnSelectAllPlayers();
                    rayStatus = RayStatus.ChooseCard;
                }
                //如果点击鼠标右键，则进入选牌状态
                if(Input.GetMouseButtonDown(1))
                {
                    UnSelectAllPlayers();
                    rayStatus = RayStatus.ChooseCard;
                }
                break;
        }
    }
    private void UnSelectAllPlayers()
    {
        foreach(var player in PlayerManager.Instance.Players)
        {
            player.Value.GetComponent<PlayerSelection>().OnUnSelect();
        }
    }
}

//悬停接口
public interface IHoverable
{
    bool IsOnHover();
    void OnHoverEnter();
    void OnHoverExit();
}