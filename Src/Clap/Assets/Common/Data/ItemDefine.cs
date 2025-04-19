using System;

namespace Common.Data
{
    public enum ItemType
    {
        攻击=-2,
        None = -1,
        货币 = 0,
        补给 = 1,
        近战 = 2,
        远程 = 3,
        防御 = 4,
        反制 = 5,
        特效 = 6,
    }

    public enum TargetType
    {
        None = -1,
        自己 = 0,
        敌方单人 = 1,
        敌方三人 = 2,
        敌方全体 = 3,
        攻击敌方 = 4,
    }


    public class ItemDefine
    {
        public int ID { get; set; } // 物品ID
        public string Name { get; set; } // 物品名称
        public string Description { get; set; } // 物品描述
        public int Order { get; set; } // 行动顺序
        public string CostType { get; set; } // 消耗类型
        public int Cost { get; set; } // 消耗数量
        public TargetType TargetType { get; set; } // 物品类型
        public ItemType ItemType { get; set; } // 物品类型
        public int ATK { get; set; } // 攻击力
        public string HurtType { get; set; } // 伤害类型
        public float Hurt { get; set; } // 伤害值
        public int[] ParryType { get; set; } // 格挡类型
        public int ParryNum { get; set; } // 格挡值
        public int[] DEFType { get; set; } // 防御类型
        public int DEFNum { get; set; } // 防御值
        public int Buff { get; set; } //对应BuffID
        public int CD { get; set; } // 使用冷却时间
        public string Audio { get; set; } // 音效路径
        public string Icon { get; set; } // 图标路径
        public string Copywriting { get; set; } // 文案

        public override string ToString()
        {
            return $"ID: {ID}, Name: {Name}, Description: {Description},CostType: {CostType}，Cost: {Cost}，"+
                   $" ItemType: {ItemType}, ATK: {ATK}, Hurt: {Hurt}, " +
                   $"ParryType: {ParryType}, ParryNum: {ParryNum}, DEFType: {DEFType}, DEFNum: {DEFNum}, " +
                   $"Buff: {Buff}, CD: {CD}, Audio: {Audio}, Icon: {Icon}, Iconbig: {Copywriting}";
        }
    }
}