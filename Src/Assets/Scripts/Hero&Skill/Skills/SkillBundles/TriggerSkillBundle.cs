/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Heroes.Skills
{
    //华雄技能耀武
    public class Courage : TriggerSkill
    {
        public Courage(): base("Courage") { }

        public override void OnDamaged(Player attacker, int damage)
        {
            attacker.status.resources.Bullet.Get(1);
            Debug.Log("受到勇气激励，获得 1 发子弹！");
        }
    }
    
    //钟馗技能亡语
    public class Deathrattle : TriggerSkill
    {
        public Deathrattle() : base("Deathrattle") { }

        public override bool OnDeath(Player self)
        {
            Debug.Log($"💀 钟馗({self.ID_inGame}) 的【死亡爆发】发动！");

            foreach (var player in PlayerManager.Instance.Players.Values)
            {
                if (player.ID_inGame == self.ID_inGame) continue;

                // 直接对所有其他玩家造成 3 点伤害
                player.status.HP.Damage(3, self, player, null);
            }
            return false;
        }
    }
    
    //妲己技能魅惑
    public class BewitchingHex : TriggerSkill
    {
        public BewitchingHex() : base("Bewitching Hex") { }

        public override void OnDamaging(Player attacker, Player victim, int damage)
        {
            victim.status.Buffs.Add(new Buff("stun", 1, 0, true));
            Debug.Log($"💫 妲己的【魅惑】发动！{victim.ID_inGame} 被眩晕 1 回合！");
        }
    }
    //诸葛亮的技能连弩
    public class RepeatingCrossbow : TriggerSkill
    {
        public RepeatingCrossbow() : base("Volley Cycle") { }

        public override void OnDamaging(Player attacker, Player victim, int damage)
        {
            attacker.status.Marks.Add("crossbow");
            int n = attacker.status.Marks.Count("crossbow");
            int multiplier = (n % 5) + 1;

            Debug.Log($"🏹 诸葛亮的【连弩】发动！当前印记数 {n}，下次伤害倍率 {multiplier}x");
        }
    }
    //扁鹊技能
    public class TerminalAffliction : TriggerSkill
    {
        public TerminalAffliction() : base("Terminal Affliction") { }

        public override void OnDamaging(Player attacker, Player victim, int damage)
        {
            // 创建中毒 Buff，持续 2 回合，每回合伤害 = 初始伤害
            Buff poison = new Buff("poison", 2, damage, isDebuff: true);
            victim.status.Buffs.Add(poison);
            Debug.Log($"☠️ 扁鹊对 {victim.ID_inGame} 造成 {damage} 点伤害并附加中毒，中毒持续 2 回合，每回合伤害 {damage}");
        }
    }
}*/