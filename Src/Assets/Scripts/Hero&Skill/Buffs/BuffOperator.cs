using UnityEngine;

public class BuffOperator : Buff
{
    /// <summary>Which step (1, 2, or 3) to add to. Step 1 & 3: additive (+). Step 2: multiplicative (*). Use negative value for -; use 1/x for /.</summary>
    public enum StepSlot { First, Second, Third }

    private float _firstAdditive;   // step 1; 0 = no step
    private float _multiplier = 1f; // step 2; 1 = no step
    private float _lastAdditive;    // step 3; 0 = no step

    public BuffOperator(string id, float value, Player owner, StepSlot step)
        : base(id, 0, false, owner)
    {
        AddStep(value, step);
    }

    /// <summary>Adds value to the given step. First/Third: add value. Second: multiply by value.</summary>
    public void AddStep(float value, StepSlot step)
    {
        switch (step)
        {
            case StepSlot.First:
                _firstAdditive += value;
                break;
            case StepSlot.Second:
                _multiplier *= value;
                break;
            case StepSlot.Third:
                _lastAdditive += value;
                break;
        }
    }

    public override bool ApplyTo(Buff existing)
    {
        if (existing is BuffOperator existingOp)
        {
            existingOp.AddStepsFrom(this);
            return true;
        }
        return false;
    }

    public void AddStepsFrom(BuffOperator other)
    {
        if (other == null) return;
        _firstAdditive += other._firstAdditive;
        _multiplier *= other._multiplier;
        _lastAdditive += other._lastAdditive;
    }

    public int ApplyOperatorInt(int originalDamage)
    {
        float current = originalDamage;
        current += _firstAdditive;
        if (_multiplier != 1f) current *= _multiplier;
        current += _lastAdditive;
        return Mathf.Max(0, Mathf.RoundToInt(current));
    }

    public float ApplyOperatorFloat(float originalDamage)
    {
        float current = originalDamage;
        current += _firstAdditive;
        if (_multiplier != 1f) current *= _multiplier;
        current += _lastAdditive;
        return current;
    }
}
