using System;

namespace Common.Data
{
    public enum ItemType
    {
        补给,
        直伤,
    }
    public enum ParryType
    {
    }
    public enum DEFType
    {
    }
    public class ItemDefine
    {
        public int ID { get; set; } // 物品ID
        public string Name { get; set; } // 物品名称
        public string Description { get; set; } // 物品描述
        public ItemType Type { get; set; } // 物品类型
        public int ATK { get; set; } // 攻击力
        public int Hurt { get; set; } // 伤害值
        public ParryType ParryType { get; set; } // 格挡类型
        public int Parry { get; set; } // 格挡值
        public DEFType DEFType { get; set; } // 防御类型
        public int DEF { get; set; } // 防御值
        public string LimitClass { get; set; } // 使用限制
        public int UseCD { get; set; } // 使用冷却时间
        public string Audio { get; set; } // 音效路径
        public string Icon { get; set; } // 图标路径

        public override string ToString()
        {
            return $"ID: {ID}, Name: {Name}, Description: {Description}, Type: {Type}, ATK: {ATK}, Hurt: {Hurt}, " +
                   $"ParryType: {ParryType}, Parry: {Parry}, DEFType: {DEFType}, DEF: {DEF}, " +
                   $"LimitClass: {LimitClass}, UseCD: {UseCD}, Audio: {Audio}, Icon: {Icon}";
        }
    }
}