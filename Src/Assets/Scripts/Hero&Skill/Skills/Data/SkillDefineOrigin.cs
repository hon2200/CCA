using System;
using System.Collections.Generic;

/// <summary>
/// Concrete data class for JSON deserialization. Matches the skill JSON schema (ID, Name, IsLimited, LimitedTimes, Description, Costs, CD).
/// </summary>
public class SkillDefineOrigin
{
    public string ID { get; set; }
    public string Name { get; set; }
    public bool IsLimited { get; set; }
    public int LimitedTimes { get; set; }
    public string Description { get; set; }
    public List<int> Costs { get; set; }
    public int CD { get; set; }
}