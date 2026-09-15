using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace Bannerlord.PartyDirectControl.Domain;

public static class DirectControl
{
    public static bool HandleInteractionWithoutTarget()
    {
        if (!ShouldControlDirectly())
        {
            return false;
        }

        SubModule.DirectControlBehavior.ApplyToPartiesUnderControl(
            party => PartyActions.EscortParty(party, MobileParty.MainParty));

        return true;
    }

    public static bool HandleInteractionWithParty(MobileParty target)
    {
        if (!ShouldControlDirectly())
        {
            return false;
        }

        var faction = Hero.MainHero.MapFaction;

        Action<MobileParty> action;
        if (FactionManager.IsAtWarAgainstFaction(faction, target.MapFaction))
        {
            action = party => PartyActions.AttackParty(party, target);
        }
        else
        {
            action = party => PartyActions.EscortParty(party, target);
        }

        SubModule.DirectControlBehavior.ApplyToPartiesUnderControl(action);

        return true;
    }

    public static bool HandleInteractionWithSettlement(Settlement target)
    {
        if (!ShouldControlDirectly())
        {
            return false;
        }

        var faction = Hero.MainHero.MapFaction;
        var targetFaction = target.MapFaction;

        Action<MobileParty> action;
        if (FactionManager.IsAtWarAgainstFaction(faction, targetFaction))
        {
            if (target.IsVillage)
            {
                action = party => PartyActions.RaidVillage(party, target);
            }
            else
            {
                action = party => PartyActions.BesiegeSettlement(party, target);
            }
        }
        else if (FactionManager.IsNeutralWithFaction(faction, targetFaction))
        {
            action = party => PartyActions.VisitSettlement(party, target);
        }
        else
        {
            // allied settlement
            if (target.IsUnderSiege)
            {
                action = party => PartyActions.DefendSettlement(party, target);
            }
            else
            {
                action = party => PartyActions.VisitSettlement(party, target);
            }
        }

        SubModule.DirectControlBehavior.ApplyToPartiesUnderControl(action);

        return true;
    }

    private static bool ShouldControlDirectly()
    {
        return Hotkeys.ModifierKey.IsHeld;
    }
}
