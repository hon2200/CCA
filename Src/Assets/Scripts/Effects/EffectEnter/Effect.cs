using UnityEngine;

public static class Effect
{
    public static void Shot(GameObject origin, GameObject target, Vector3 offset = new Vector3(), string effectName ="")
    {
        switch (effectName)
        {
            default:
                EffectManager.Instance.Shot("Bullet", origin, target, offset);
                break;
        }
    }

    public static void Defend(GameObject target, Vector3 offset = new Vector3(), string effectName = "")
    {
        switch (effectName)
        {
            default:
                EffectManager.Instance.Defend("Shield", target, offset);
                break;
        }
    }

    public static void Hit(GameObject target, Vector3 offset = new Vector3(), string effectName = "")
    {
        switch (effectName)
        {
            default:
                EffectManager.Instance.Hit("Hit", target, offset);
                break;
        }
    }
}