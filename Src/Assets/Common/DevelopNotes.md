### 代码架构方面

#### 底层逻辑

1.Log Nest函数，对变量的名字很依赖

2.可能有很多地方没有写get和set，但不影响功能，其次，有很多地方也不能写

```C#
public int ID_inGame { get; set; }
public PlayerStatus status { get; set; }
public PlayerAction action { get; set; }
public PlayerUI playerUI;
public void Initialize(int ID_inGame, PlayerStatus status, PlayerAction action)
{
    this.ID_inGame = ID_inGame;
    this.status = status;
    this.action = action;
}
```

这个地方写的巨丑，感觉不写成字段反而方便，不然还要System.Serializable

3.激光炮和核弹的结算位置是拙劣的，未来需要修改

4.每个玩家要一个ActionDataBase，或者至少有某种替代方案，未来需要修改

5.以int类型存储的卡牌ID，可读性很低，我决定牺牲性能来把卡牌的ID改成string，其实感觉也牺牲不了多少

6.每一个Player拥有一个ActionDeck，是ActionDefine的子集，CardLiberary就不用改了，CardTemplete里面的内容基本上是不会变的

deepseek强烈建议改成string，同时还建议我加入各种前后缀（例如attack_nuclearbomb_explosion)，我认为现阶段没有必要

--已经解决

7.考虑过将OnHoverEnter等一系列函数写成委托，但是我觉得，首先这个函数是得指定Card调用的，这就意味着委托放在CardSelectionManager会带来引用的不变，同时这些效果并不会复用，那么也就意味着CardSelection这个类，地位暂时不可被替代，那么我认为没有必要再写委托了。

#### 交互逻辑

1.卡牌信息展示：想要防御是一个展开的方式，但是目前懒的写了

2.ActionDataBase需要删掉ActionType、TargetType，把它调整到Templete，交互层吗？

3.创建人类玩家的代码我还没想好，甚至连是不是要预先创建好我都还没想好

4.Scene的组织还不是很有序



#### 其他

1.需要调查一下Addressable Asset怎么使用



### 开发任务

#### 未来开发计划

1.先完成卡牌的生成，

玩家输入逻辑 Onclick，OnPointEnter

2.AI输入逻辑

输入：玩家的其他状态

输出：这回合行动

-2周

3.死亡判定，游戏结束判定并且与Debug模式和教程模式进行衔接

-2周

4.加入所有教程模式并且进行调试

-2周

#### 改进

新书：

- 鼠标移动上去字体产生浮动
- 换一个漂亮的字体

3.可以把获取鼠标位置的功能集合成函数







#### Reference

[Unity实现杀戮尖塔出牌效果（一.扇形开牌）_unity 拖动卡牌-CSDN博客](https://blog.csdn.net/qq_33894287/article/details/136405209)



Phase : 有来自于各个Phase的继承子类

Phase.OnEnteringPhase()，在进入时触发，

OnExisiting()，触发并且进入下一个Phase

每一个Phase定义一个Ready，一旦ok执行OnExisting？



ReadyDetector：

```c#
List<bool> ready;
bool allReady;
```





每一帧都执行：



Player

{

}



点击开始游戏：

进入开始阶段，执行开始阶段，玩家确认后：

进入行动阶段，玩家确认执行行动后



































