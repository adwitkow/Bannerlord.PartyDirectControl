using HarmonyLib;
using HarmonyLib.PatchBuilder;
#if !LOWER_THAN_1_3
using NavalDLC.View.Map.Visuals;
using SandBox.View.Map.Visuals;
#endif
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace Bannerlord.PartyDirectControl.Patches;

public class DirectCommandPatches
{
    public static void Apply(Harmony harmony)
    {
#if LOWER_THAN_1_3
        harmony.Patch<MobilePartyAi>()
            .Method(x => x.SetMoveGoToSettlement(default))
                .Prefix(MobilePartyAi_SetMoveGoToSettlement)
            .Method(x => x.SetMoveEscortParty(default))
                .Prefix(MobilePartyAi_SetMoveEscortParty)
            .Method(x => x.SetMoveEngageParty(default))
                .Prefix(MobilePartyAi_SetMoveEngageParty)
            .Method(x => x.SetMoveGoToPoint(default))
                .Prefix(MobilePartyAi_SetMoveGoToPoint);
#else
        harmony.Patch<MobilePartyVisual>()
            .Method(x => x.OnMapClick(default))
                .Prefix(MobilePartyVisual_OnMapClick);
        harmony.Patch<NavalMobilePartyVisual>()
            .Method(x => x.OnMapClick(default))
                .Prefix(MobilePartyVisual_OnMapClick);
        harmony.Patch<SettlementVisual>()
            .Method(x => x.OnMapClick(default))
                .Prefix(SettlementVisual_OnMapClick);
        harmony.Patch<MobileParty>()
            .Method(x => x.SetMoveGoToPoint(default, default))
                .Prefix(MobileParty_SetMoveGoToPoint);
#endif
    }

#if LOWER_THAN_1_3
    public static bool MobilePartyAi_SetMoveEscortParty(ref MobilePartyAi __instance, MobileParty party)
    {
        if (!ShouldControlDirectly() || __instance != MobileParty.MainParty.Ai)
        {
            return true;
        }

        CommandInteractionWithParty(party);

        return false;
    }

    public static bool MobilePartyAi_SetMoveEngageParty(ref MobilePartyAi __instance, MobileParty party)
    {
        if (!ShouldControlDirectly() || __instance != MobileParty.MainParty.Ai)
        {
            return true;
        }

        CommandInteractionWithParty(party);

        return false;
    }

    public static bool MobilePartyAi_SetMoveGoToSettlement(ref MobilePartyAi __instance, Settlement settlement)
    {
        if (!ShouldControlDirectly() || __instance != MobileParty.MainParty.Ai)
        {
            return true;
        }

        CommandInteractionWithSettlement(settlement);

        return false;
    }

    public static bool MobilePartyAi_SetMoveGoToPoint(ref MobilePartyAi __instance)
    {
        if (!ShouldControlDirectly() || __instance != MobileParty.MainParty.Ai)
        {
            return true;
        }

        SubModule.DirectControlBehavior.ApplyToPartiesUnderControl(
            party => PartyActions.EscortParty(party, MobileParty.MainParty));

        return false;
    }
#else
    public static bool MobilePartyVisual_OnMapClick(ref MobilePartyVisual __instance, ref bool __result)
    {
        if (!ShouldControlDirectly())
        {
            return true;
        }

        CommandInteractionWithParty(__instance.MapEntity.MobileParty);

        return false;
    }

    public static bool NavalMobilePartyVisual_OnMapClick(ref NavalMobilePartyVisual __instance, ref bool __result)
    {
        if (!ShouldControlDirectly())
        {
            return true;
        }

        CommandInteractionWithParty(__instance.MapEntity.MobileParty);

        return false;
    }

    public static bool SettlementVisual_OnMapClick(ref SettlementVisual __instance, ref bool __result)
    {
        if (!ShouldControlDirectly())
        {
            return true;
        }

        CommandInteractionWithSettlement(__instance.MapEntity.Settlement);
        
        return false;
    }

    public static bool MobileParty_SetMoveGoToPoint()
    {
        if (!ShouldControlDirectly())
        {
            return true;
        }

        SubModule.DirectControlBehavior.ApplyToPartiesUnderControl(
            party => PartyActions.EscortParty(party, MobileParty.MainParty));

        return false;
    }
#endif

    private static void CommandInteractionWithParty(MobileParty target)
    {
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
    }

    private static void CommandInteractionWithSettlement(Settlement target)
    {
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
    }

    private static bool ShouldControlDirectly()
    {
        return Hotkeys.ModifierKey.IsHeld;
    }
}