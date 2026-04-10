using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Hero skill templates. Data (Name, Description, Costs, CD, etc.) is loaded from HeroSkill.json by ID.
// Concrete logic to be implemented as needed.

public class Peerless : HeroSkill, IPhaseEnterHandler, IPhaseExitHandler
{
    private AttackingLevelOperator _peerlessAtkBuff;

    public Peerless() : base("Peerless") { }

    public void OnPhase(Phase phase)
    {
        if (phase is StartPhase && Owner != null)
        {
            int n = PlayerManager.Instance.AlivePlayerNumber;
            float value = 0.5f * n;
            _peerlessAtkBuff = new AttackingLevelOperator(value, Owner, BuffOperator.StepSlot.Third);
            Owner.status.buffs.Apply(_peerlessAtkBuff);
        }
    }

    public void ExitingPhase(Phase phase)
    {
        if (phase is EndPhase && Owner != null && _peerlessAtkBuff != null)
        {
            Owner.status.buffs.Remove(_peerlessAtkBuff, "PeerlessEndTurn");
            _peerlessAtkBuff = null;
        }
    }
}

public class TripleTributeOath : HeroSkill, IPhaseEnterHandler
{
    public TripleTributeOath() : base("Triple Tribute Oath") { }

    public void OnPhase(Phase phase) 
    {
        if (BattleManager.Instance.Turn.Value == 1)
        {
            Owner.status.resources.Bullet.Get(3);
        }
    }
}

public class DecadeDominance : HeroSkill
{
    private DamagingOperator _decadeDominanceBuff;
    private bool _isUsed;

    public DecadeDominance() : base("Decade's Dominance") { }

    protected override void Envoke() { }

    public override void OnSetHero()
    {
        float multiplier = Mathf.Max(1f, Owner.status.HP.Value / 10f);
        _decadeDominanceBuff = new DamagingOperator(multiplier, Owner, BuffOperator.StepSlot.Second);
        Owner.status.buffs.Apply(_decadeDominanceBuff);
        _isUsed = true;
    }

    public override void OnDisabled()
    {
        if (Owner != null && _decadeDominanceBuff != null)
        {
            Owner.status.buffs.Remove(_decadeDominanceBuff, "DecadeDominanceDisabled");
            _decadeDominanceBuff = null;
        }
    }
}

public class ThespianCurse : HeroSkill
{
    public ThespianCurse() : base("Thespian's Curse") { }
}

public class MountainCrusher : HeroSkill
{
    private DamagingOperator _mountainCrusherDmgBuff;
    bool isUsed = false;

    public MountainCrusher() : base("Mountain Crusher") { }
    protected override void Envoke() { }

    public override void OnSetHero()
    {
        _mountainCrusherDmgBuff = new DamagingOperator(3, Owner, BuffOperator.StepSlot.Second);
        Owner.status.buffs.Apply(_mountainCrusherDmgBuff);
        isUsed = true;
    }

    public override void OnDisabled()
    {
        if (Owner != null && _mountainCrusherDmgBuff != null)
        {
            Owner.status.buffs.Remove(_mountainCrusherDmgBuff, "MountainCrusherDisabled");
            _mountainCrusherDmgBuff = null;
        }
    }
}

public class RoyalKnight : HeroSkill
{
    public RoyalKnight() : base("Royal Knight") { }
}

public class EXcaliburBigSword : HeroSkill
{
    public EXcaliburBigSword() : base("EXcalibur!") { }
}

public class BladeDominator : HeroSkill
{
    public BladeDominator() : base("Blade Dominator") { }
}

public class ChainDeceit : HeroSkill
{
    public ChainDeceit() : base("Chain Deceit") { }
}

public class PandoraTorrent : HeroSkill
{
    public PandoraTorrent() : base("Pandora's Torrent") { }
}

public class PlagueReckoning : HeroSkill
{
    public PlagueReckoning() : base("Plague Reckoning") { }
}

public class StabVolley : HeroSkill
{
    public StabVolley() : base("Stab Volley") { }
}

public class SacrificialFury : HeroSkill
{
    public SacrificialFury() : base("Sacrificial Fury") { }
}

public class VindictiveScript : HeroSkill
{
    public VindictiveScript() : base("Vindictive Script") { }
}

public class DeathDecree : HeroSkill
{
    public DeathDecree() : base("Death Decree") { }
}

public class RetributionOfEmpress : HeroSkill
{
    public RetributionOfEmpress() : base("Retribution of Empress") { }
}

public class CelestialShatter : HeroSkill
{
    public CelestialShatter() : base("Celestial Shatter") { }
}

public class BewitchingHexHero : HeroSkill
{
    public BewitchingHexHero() : base("Bewitching Hex") { }
}

public class RoyalBarricade : HeroSkill
{
    public RoyalBarricade() : base("Royal Barricade") { }
}

public class AlchemicSynthesis : HeroSkill
{
    public AlchemicSynthesis() : base("Alchemic Synthesis") { }
}
