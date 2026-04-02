using UnityEngine;

/// <summary>
/// Runtime bridge between <see cref="HeroDefine"/> (data) and <see cref="HeroTemplete"/> (prefab / UI art).
/// Attach to the hero offer prefab alongside your UI scripts.
/// </summary>
public class RuntimeHero : MonoBehaviour
{
    public HeroTemplete heroTemplete;
    public HeroDefine heroDefine;
}
