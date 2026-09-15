#if LOWER_THAN_1_3
using Bannerlord.PartyDirectControl.Domain;
using HarmonyLib;
using HarmonyLib.PatchBuilder;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace Bannerlord.PartyDirectControl.Patches;

public partial class DirectCommandPatches
{
    private static void ApplyForLowerThan1_3(Harmony harmony)
    {
        harmony.Patch<MobilePartyAi>()
            .Method(x => x.SetMoveGoToSettlement(default))
                .Prefix(MobilePartyAi_SetMoveGoToSettlement)
            .Method(x => x.SetMoveEscortParty(default))
                .Prefix(MobilePartyAi_SetMoveEscortParty)
            .Method(x => x.SetMoveEngageParty(default))
                .Prefix(MobilePartyAi_SetMoveEngageParty)
            .Method(x => x.SetMoveGoToPoint(default))
                .Prefix(MobilePartyAi_SetMoveGoToPoint);
    }

    public static bool MobilePartyAi_SetMoveEscortParty(ref MobilePartyAi __instance, MobileParty party)
    {
        if (__instance != MobileParty.MainParty.Ai)
        {
            return true;
        }

        var handled = DirectControl.HandleInteractionWithParty(party);

        return !handled;
    }

    public static bool MobilePartyAi_SetMoveEngageParty(ref MobilePartyAi __instance, MobileParty party)
    {
        if (__instance != MobileParty.MainParty.Ai)
        {
            return true;
        }

        var handled = DirectControl.HandleInteractionWithParty(party);

        return !handled;
    }

    public static bool MobilePartyAi_SetMoveGoToSettlement(ref MobilePartyAi __instance, Settlement settlement)
    {
        if (__instance != MobileParty.MainParty.Ai)
        {
            return true;
        }

        var handled = DirectControl.HandleInteractionWithSettlement(settlement);

        return !handled;
    }

    public static bool MobilePartyAi_SetMoveGoToPoint(ref MobilePartyAi __instance)
    {
        if (__instance != MobileParty.MainParty.Ai)
        {
            return true;
        }

        var handled = DirectControl.HandleInteractionWithoutTarget();

        return !handled;
    }
}
#endif