using System;
using UnityEngine.InputSystem.Utilities;

public class GoldAttribute : ObservableAttribute<int>
{
    public void GetGold(int income) => SetValue(Value + income, "Get");
    public void LoseGold(int loss) => SetValue(Value - loss, "Lose");
    public void SetGold(int gold) => SetValue(gold, "Set");
}
