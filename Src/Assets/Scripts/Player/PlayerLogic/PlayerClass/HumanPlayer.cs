using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HumanPlayer : Player
{
    //人类玩家做好准备，你的诞生要让很多东西都做好准备
    public void HumanPlayerListenUp()
    {
        CardSelectionManager.Instance.player1 = this;
        CardDemonstrateSystem.Instance.AddListener(this);
        CardPresentSystem.Instance.player1 = this;
        RoundMonitor.Instance.player1 = this;
        CardPresentSystem.Instance.CreateAndArrangeCards();
    }
    //创建英雄模式的玩家
    public void InitializePlayer(int ID_inGame, HeroDefine heroDefine, TutorialDefine Level = null)
    {
        var initialResource = new List<int> { 0, 0, 0 };
        List<string> availableActionIds;
        if (Level != null)
            availableActionIds = Level.GetAllUnlockedActions();
        else if (RougeManager.Instance != null && RougeManager.Instance.rougePlayer != null)
            availableActionIds = RougeManager.Instance.rougePlayer.GetAvailableBattleActionIdsOrDefault();
        else
            availableActionIds = RougePlayer.GetDefaultBattleAvailableActionIds();
        base.Initialize(ID_inGame, "Player", PlayerType.Human, heroDefine.MaxHP,
            initialResource, availableActionIds, heroDefine.ID, heroDefine.SkillIDList);
        OnBirth?.Invoke();
        HumanPlayerListenUp();
    }
}
