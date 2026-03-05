using System;

/// <summary>
/// Enemy data loaded from Enemy.json. Extends HeroDefine with EnemyType (Boss, Minion, Elite).
/// </summary>
[Serializable]
public class EnemyDefine : HeroDefine
{
    public string EnemyType;

    public EnemyDefine() { }
}
