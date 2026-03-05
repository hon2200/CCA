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

    /// <summary>Steps in order; used when merging BuffOperators.</summary>
    public IReadOnlyList<Step> Steps => _operators;

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

    public void AddStep(OpType op, float operand) => AddStepWithCancellation(op, operand);

    /// <summary>
    /// Adds a step with cancellation: same-tier steps merge (e.g. *3 then /2 → *1.5; +5 then -3 → +2).
    /// If the last step is * or / and the new step is + or -, the new step is ignored.
    /// </summary>
    private void AddStepWithCancellation(OpType op, float operand)
    {
        if (_operators.Count == 0)
        {
            _operators.Add(new Step(op, operand));
            return;
        }

        var last = _operators[^1];

        // Last is Multiply or Divide, new is Add or Subtract → do nothing
        if ((last.Op == OpType.Multiply || last.Op == OpType.Divide) && (op == OpType.Add || op == OpType.Subtract))
            return;

        // Add/Subtract with Add/Subtract → merge into one step
        if (last.Op == OpType.Add && op == OpType.Add) { _operators[^1] = new Step(OpType.Add, last.Operand + operand); return; }
        if (last.Op == OpType.Add && op == OpType.Subtract) { _operators[^1] = new Step(OpType.Add, last.Operand - operand); return; }
        if (last.Op == OpType.Subtract && op == OpType.Add) { _operators[^1] = new Step(OpType.Subtract, last.Operand - operand); return; }
        if (last.Op == OpType.Subtract && op == OpType.Subtract) { _operators[^1] = new Step(OpType.Subtract, last.Operand + operand); return; }

        // Multiply/Divide with Multiply/Divide → merge into one Multiply step where possible
        if (last.Op == OpType.Multiply && op == OpType.Multiply) { _operators[^1] = new Step(OpType.Multiply, last.Operand * operand); return; }
        if (last.Op == OpType.Multiply && op == OpType.Divide) { _operators[^1] = new Step(OpType.Multiply, operand != 0 ? last.Operand / operand : last.Operand); return; }
        if (last.Op == OpType.Divide && op == OpType.Multiply) { _operators[^1] = new Step(OpType.Multiply, last.Operand != 0 ? operand / last.Operand : operand); return; }
        if (last.Op == OpType.Divide && op == OpType.Divide) { _operators[^1] = new Step(OpType.Divide, last.Operand * operand); return; }

        _operators.Add(new Step(op, operand));
    }

    /// <summary>
    /// Merge by appending the new BuffOperator's steps to the existing list. Returns true when existing is a BuffOperator.
    /// </summary>
    public override bool ApplyTo(Buff existing)
    {
        if (existing is BuffOperator existingOp)
        {
            existingOp.AddStepsFrom(this);
            return true;
        }
        return false;
    }

    /// <summary>Appends all steps from another BuffOperator with cancellation applied per step.</summary>
    public void AddStepsFrom(BuffOperator other)
    {
        if (other == null) return;
        foreach (var step in other.Steps)
            AddStepWithCancellation(step.Op, step.Operand);
    }

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