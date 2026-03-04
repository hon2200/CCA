using System.Collections.Generic;
using UnityEngine;

public class BuffOperator : Buff
{
    public enum OpType { Add, Subtract, Multiply, Divide }

    /// <summary>Single step: apply one operator with an operand. Order in the list matters.</summary>
    public struct Step
    {
        public OpType Op;
        public float Operand;
        public Step(OpType op, float operand) { Op = op; Operand = operand; }
    }

    private readonly List<Step> _operators = new();

    public BuffOperator(string id, List<Step> operators, Player owner)
        : base(id, 0, false, owner)
    {
        if (operators != null)
            _operators.AddRange(operators);
    }
    public BuffOperator(string id, Step operator1, Player owner)
        : base("DamageOperator", 0, false, owner)
    {
        _operators.Add(operator1);
    }

    public void AddStep(OpType op, float operand) => _operators.Add(new Step(op, operand));

    /// <summary>
    /// Applies the operator list to damage in sequence. Order matters (e.g. +5 then *2 vs *2 then +5).
    /// Returns the final damage, clamped to non-negative integer.
    /// </summary>
    public int ApplyOperatorInt(int originalDamage)
    {
        float current = originalDamage;
        foreach (var step in _operators)
        {
            switch (step.Op)
            {
                case OpType.Add:
                    current += step.Operand;
                    break;
                case OpType.Subtract:
                    current -= step.Operand;
                    break;
                case OpType.Multiply:
                    current *= step.Operand;
                    break;
                case OpType.Divide:
                    current = step.Operand != 0 ? current / step.Operand : current;
                    break;
            }
        }
        return Mathf.Max(0, Mathf.RoundToInt(current));
    }
    public float ApplyOperatorFloat(float originalDamage)
    {
        float current = originalDamage;
        foreach (var step in _operators)
        {
            switch (step.Op)
            {
                case OpType.Add:
                    current += step.Operand;
                    break;
                case OpType.Subtract:
                    current -= step.Operand;
                    break;
                case OpType.Multiply:
                    current *= step.Operand;
                    break;
                case OpType.Divide:
                    current = step.Operand != 0 ? current / step.Operand : current;
                    break;
            }
        }
        return current;
    }
}