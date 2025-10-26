using UnityEngine;

// all skills need ui logic
namespace Heroes.Skills
{
    public class QuickSupply : PhasebasedSkill
    {
        public QuickSupply()
        {
            ID = "QuickSupply";
        }

        public override void OnPhaseStart(Player thisPlayer)
        {
            base.OnPhaseStart(thisPlayer);
            if (thisPlayer.status.resources.DreamWill.Value >= 1)
            {
                thisPlayer.status.resources.DreamWill.Use(1);
                thisPlayer.status.resources.Bullet.Get(1);
                Debug.Log("{thisPlayer.hero.ID} 使用【快速补给】，消耗1点梦意志，获得1点子弹。");
            }
            else
            {
                Debug.Log("{thisPlayer.hero.ID} 尝试使用【快速补给】，但梦意志不足。");
            }
        }
    }

    public class CandleSanctuary : PhasebasedSkill
    {
        private bool hasActivated = false;

        public CandleSanctuary()
        {
            ID = "CandleSanctuary";
        }

        public override void OnPhaseStart(Player thisPlayer)
        {
            if (hasActivated || thisPlayer.status.resources.DreamWill.Value < 1)
                return;
            thisPlayer.status.resources.DreamWill.Use(1);

            //add invincible buff to all friends

            hasActivated = true;

            Debug.Log("【烛辉庇护】已发动！所有单位获得 1 回合无敌。");
        }
        
    }

    public class Feedback
    {
        public class Feedbackphased : PhasebasedSkill
        {
            private bool hasActivated = false;   // 是否已经发动过（锁定技只能发动一次）

            public Feedbackphased()
            {
                ID = "Feedback";
            }

            // 第一阶段：在“结束阶段”时检测
            public override void OnPhaseEnd(Player thisPlayer)
            {
                if (!hasActivated && thisPlayer.status.resources.DreamWill.Value >= 2)
                {
                    // 消耗梦意志
                    thisPlayer.status.resources.DreamWill.Use(2);

                    // 输出日志
                    Debug.Log("{thisPlayer.hero.ID} 在结束阶段发动【反馈】，消耗2点梦意志。");

                    // 添加触发技 FeedbackTrigger
                    var triggerSkill = new FeedbackTrigger(thisPlayer);

                    Debug.Log("{thisPlayer.hero.ID} 的【反馈】效果激活");
                    hasActivated = true;
                }
                else 
                {
                    Debug.Log($"{thisPlayer.hero.ID} 尝试发动【反馈】，但失败");
                }
            }
        }

// 第二阶段：触发技部分
        public class FeedbackTrigger : TriggerSkill
        {
            
            public FeedbackTrigger(Player player)
            {
                this.ID = "FeedbackTrigger";
            }

            // 每次受到伤害时触发
            public override void OnDamaged(Player attacker, int damage)
            {
                //获得1点与伤害来源相同类型的资源
            }
        }
    }
    

}