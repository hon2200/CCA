using UnityEngine;

/// <summary>
/// Runtime bridge between RelicDefine (data) and RelicTemplete (UI assets).
/// Attach to the same GameObject as RelicUI. Set relicTemplete and relicDefine when creating the relic display.
/// </summary>
public class RuntimeRelic : MonoBehaviour
{
    public RelicTemplete relicTemplete;
    public RelicDefine relicDefine;
}
