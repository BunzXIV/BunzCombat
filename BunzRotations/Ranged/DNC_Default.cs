namespace BunzRotations.Ranged;

[SourceCode(Path = "main/BunzRotations/Ranged/DNC_Default.cs")]
public sealed class DNC_Default : DNC_Base
{
    #region General rotation info
    public override string GameVersion => VERSION;
    public override string RotationName => $"{USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    public override CombatType Type => CombatType.PvE;
    #endregion General rotation info

    #region Rotation Configs
    // N/A
    #endregion

    #region Countdown logic

    protected override IAction CountDownAction(float remainTime)
    {
        //if(remainTime <= CountDownAhead)
        //{
        //    if(DanceFinishGCD(out))
        //}
        if (remainTime <= 15)
        {
            if (StandardStep.CanUse(out var act, CanUseOption.MustUse)) return act;
            if (ExecuteStepGCD(out act)) return act;
        }
        return base.CountDownAction(remainTime);
    }

    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction act)
    {
        if (!InCombat && !Player.HasStatus(true, StatusID.ClosedPosition) && ClosedPosition.CanUse(out act)) return true;

        if (DanceFinishGCD(out act)) return true;
        if (ExecuteStepGCD(out act)) return true;

        if (IsBurst && InCombat && TechnicalStep.CanUse(out act, CanUseOption.MustUse)) return true;

        if (AttackGCD(out act, Player.HasStatus(true, StatusID.Devilment))) return true;

        return base.GeneralGCD(out act);
    }

    private static bool AttackGCD(out IAction act, bool breaking)
    {
        act = null;
        if (IsDancing) return false;

        if ((breaking || Esprit >= 85) && SaberDance.CanUse(out act, CanUseOption.MustUse)) return true;

        if (StarFallDance.CanUse(out act, CanUseOption.MustUse)) return true;

        if (Tillana.CanUse(out act, CanUseOption.MustUse)) return true;

        if (UseStandardStep(out act)) return true;

        if (BloodShower.CanUse(out act)) return true;
        if (FountainFall.CanUse(out act)) return true;

        if (RisingWindmill.CanUse(out act)) return true;
        if (ReverseCascade.CanUse(out act)) return true;

        if (BladeShower.CanUse(out act)) return true;
        if (Windmill.CanUse(out act)) return true;

        if (Fountain.CanUse(out act)) return true;
        if (Cascade.CanUse(out act)) return true;

        return false;
    }

    private static bool UseStandardStep(out IAction act)
    {
        if (!StandardStep.CanUse(out act, CanUseOption.MustUse)) return false;
        if (Player.WillStatusEndGCD(2, 0, true, StatusID.StandardFinish)) return true;

        if (!HasHostilesInRange) return false;

        if (TechnicalStep.EnoughLevel && (Player.HasStatus(true, StatusID.TechnicalFinish) || TechnicalStep.IsCoolingDown && TechnicalStep.WillHaveOneCharge(5))) return false;

        return true;
    }

    private static bool UseClosedPosition(out IAction act)
    {
        if (!ClosedPosition.CanUse(out act)) return false;

        if (InCombat && Player.HasStatus(true, StatusID.ClosedPosition))
        {
            foreach (var friend in PartyMembers)
            {
                if (friend.HasStatus(true, StatusID.ClosedPosition2))
                {
                    if (ClosedPosition.Target != friend) return true;
                    break;
                }
            }
        }
        return false;
    }
    #endregion

    #region oGCD Logic

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (IsDancing)
        {
            return base.EmergencyAbility(nextGCD, out act);
        }

        if (TechnicalStep.ElapsedAfter(115)
            && UseBurstMedicine(out act)) return true;

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(out IAction act)
    {
        act = null;
        if (IsDancing) return false;

        if (Devilment.CanUse(out act))
        {
            if (IsBurst && !TechnicalStep.EnoughLevel) return true;

            if (Player.HasStatus(true, StatusID.TechnicalFinish)) return true;
        }

        if (UseClosedPosition(out act)) return true;

        if (Flourish.CanUse(out act)) return true;
        if (FanDance3.CanUse(out act, CanUseOption.MustUse)) return true;

        if (Player.HasStatus(true, StatusID.Devilment) || Feathers > 3 || !TechnicalStep.EnoughLevel)
        {
            if (FanDance2.CanUse(out act)) return true;
            if (FanDance.CanUse(out act)) return true;
        }

        if (FanDance4.CanUse(out act, CanUseOption.MustUse))
        {
            if (TechnicalStep.EnoughLevel && TechnicalStep.IsCoolingDown && TechnicalStep.WillHaveOneChargeGCD()) return false;
            return true;
        }

        return base.AttackAbility(out act);
    }

    #endregion

    #region Extra Methods

    #endregion
}
