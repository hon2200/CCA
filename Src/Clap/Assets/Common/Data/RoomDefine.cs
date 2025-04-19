using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Common.Data
{
    public enum Privacy//公开权限
    {
        Public =0,
        Private =1,
        CanJoin=2,
    }

    public enum Move
    { 
    }
    public enum Reward
    {
    }


    public class RoomDefine
    {     //部分内容可酌情合并
        public int ID { get; set; } // 房间ID
        public string Name { get; set; } // 房间名称
        public string Description { get; set; }//房间描述
        public int NumberCount { get; set; } // 房间人数
        public string PassWord { get; set; } // 房间密码
        public int MasterID { get; set; } // 房主ID
        public string MasterName { get; set; } // 房主名称

        public List<int> NumberID { get; set; } = new(); // 房间成员ID
        public Dictionary<int,Hero> HeroList { get; set; } = new();// 人物表，用于历史查看
        public Dictionary<int, Move> MoveList { get; set; } = new(); // 行动表，用于历史查看
        public Dictionary<int, Reward> rewardList { get; set; } = new(); // 收益表，用于历史查看
        public int AIType { get; set; } // 人机难度，例如团战有人中途退出，人机接管
        public Privacy isPublic { get; set; } // 是否公开
        public override string ToString()
        {
            string numberIDStr = NumberID != null ? string.Join(", ", NumberID.Cast<object>()) : "null";
            string heroListStr = HeroList != null ? string.Join(", ", HeroList.Cast<object>()) : "null";
            string moveListStr = MoveList != null ? string.Join(", ", MoveList.Cast<object>()) : "null";
            string rewardListStr = rewardList != null ? string.Join(", ", rewardList.Cast<object>()) : "null";

            return $"ID: {ID}, " +
                   $"Name: {Name}, " +
                   $"Description: {Description}, " +
                   $"NumberCount: {NumberCount}, " +
                   $"PassWord: {PassWord}, " +
                   $"MasterID: {MasterID}, " +
                   $"MasterName: {MasterName}, " +
                   $"NumberID: [{numberIDStr}], " +
                   $"HeroList: [{heroListStr}], " +
                   $"MoveList: [{moveListStr}], " +
                   $"RewardList: [{rewardListStr}], " +
                   $"AIType: {AIType}, " +
                   $"isPublic: {isPublic}";
        }

        
    }
}