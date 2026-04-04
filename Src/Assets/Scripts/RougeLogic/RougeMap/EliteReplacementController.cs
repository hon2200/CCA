using UnityEngine;

[System.Serializable]
public class EliteReplacementController
{
    [Header("Minion -> Elite replacement")]
    [Range(0f, 1f)] public float targetEliteReplacementRate = 0.33f;
    [Range(0f, 1f)] public float minEliteReplacementChance = 0f;
    [Range(0f, 1f)] public float maxEliteReplacementChance = 1f;
    [Tooltip("Chance increase after a failed replacement attempt.")]
    [Range(0f, 1f)] public float replacementChanceRiseOnFail = 0.05f;
    [Tooltip("Chance decrease after a successful replacement.")]
    [Range(0f, 1f)] public float replacementChanceFallOnSuccess = 0.1f;

    [Tooltip("Optional telemetry in inspector.")]
    public int eliteReplacementAttempts = 0;
    public int eliteReplacementSuccesses = 0;
    [SerializeField] private float eliteReplacementPressure = 0f;

    public void ResetRunState()
    {
        eliteReplacementPressure = 0f;
        eliteReplacementAttempts = 0;
        eliteReplacementSuccesses = 0;
    }

    public bool ShouldTryEliteReplacement(out float roll, out float chance)
    {
        chance = Mathf.Clamp(
            targetEliteReplacementRate + eliteReplacementPressure,
            minEliteReplacementChance,
            maxEliteReplacementChance);
        roll = Random.value;
        return roll < chance;
    }

    public void RecordResult(bool success)
    {
        eliteReplacementAttempts++;
        if (success)
            eliteReplacementSuccesses++;

        if (success)
            eliteReplacementPressure -= replacementChanceFallOnSuccess;
        else
            eliteReplacementPressure += replacementChanceRiseOnFail;
    }
}
