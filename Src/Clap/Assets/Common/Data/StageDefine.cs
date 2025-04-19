using JetBrains.Annotations;
using System;

namespace Common.Data
{
    public enum LevelType 
    {
        None = 0,

    }

    public enum LimitType
    {
        None = 0,

    }

    public enum GameLevel
    { 
        None = 0,
    }


    public class StageDefine
    {     //部分内容可酌情合并
        public int ID { get; set; } // 关卡ID
        public string Name { get; set; } // 关卡名称
        public string Description { get; set; } // 关卡描述
        public ItemType Type { get; set; } // 关卡模式?禁用卡牌组
        public Array enemy { get; set; }//
        public GameLevel gameLevel { get; set; } // 人机难度
        public Array rewardType { get; set; } // 奖励类型？
        public Array rewardCount { get; set; } // 奖励数量？
        public Array Pretask { get; set; } // 前置任务，采用数组，可方便管理多结局走向
        public Array Posttask { get; set; } // 后置任务，采用数组，可方便管理多结局走向
        public bool isRepeat { get; set; } // 是否可以重复进入
        public bool isFinish { get; set; } // 完成情况
        public int scores { get; set; } // 得分

        //以下分情况选择是否弃用

        public int Number { get; set; } // 人数限制
        public int Tasknodes { get; set; }//任务节点
        public int CurrentNode { get; set; }//当前节点


        //同时可以考虑是否和RoomDefine合并，此外如果需要对于其他类型的一些限制，比如说等级，是否拥有好友，等等，可以考虑
        public override string ToString()
        {
            return $"ID: {ID}, " +
                   $"Name: {Name}, " +
                   $"Description: {Description}, " +
                   $"HurtType: {Type}, " +
                   $"GameLevel: {gameLevel}, " +
                   $"RewardType: {rewardType}, " +
                   $"RewardCount: {rewardCount}, " +
                   $"Pretask: {Pretask}, " +
                   $"Posttask: {Posttask}, " +
                   $"IsRepeat: {isRepeat}, " +
                   $"IsFinish: {isFinish}, " +
                   $"Scores: {scores}, " +
                   $"Number: {Number}, " +
                   $"Tasknodes: {Tasknodes}, " +
                   $"CurrentNode: {CurrentNode}";
        }
    }
}