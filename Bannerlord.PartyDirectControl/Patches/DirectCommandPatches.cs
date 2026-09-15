using HarmonyLib;
#if !LOWER_THAN_1_3
using Bannerlord.PartyDirectControl.Domain;
using HarmonyLib.PatchBuilder;
using SandBox.View.Map.Visuals;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.ModuleManager;
#endif

namespace Bannerlord.PartyDirectControl.Patches;

public partial class DirectCommandPatches
{
    public static void Apply(Harmony harmony)
    {
#if LOWER_THAN_1_3
        ApplyForLowerThan1_3(harmony);
#else
        harmony.Patch<MobilePartyVisual>()
            .Method(x => x.OnMapClick(default))
                .Prefix(MobilePartyVisual_OnMapClick);
        harmony.Patch<SettlementVisual>()
            .Method(x => x.OnMapClick(default))
                .Prefix(SettlementVisual_OnMapClick);
        harmony.Patch<MobileParty>()
            .Method(x => x.SetMoveGoToPoint(default, default))
                .Prefix(MobileParty_SetMoveGoToPoint);

        if (ModuleHelper.IsModuleActive("NavalDLC"))
        {
            ApplyForNavalDlc(harmony);
        }
#endif
    }

#if !LOWER_THAN_1_3
    public static bool MobilePartyVisual_OnMapClick(ref MobilePartyVisual __instance, ref bool __result)
    {
        var handled = DirectControl.HandleInteractionWithParty(__instance.MapEntity.MobileParty);

        return !handled;
    }

    public static bool SettlementVisual_OnMapClick(ref SettlementVisual __instance, ref bool __result)
    {
        var handled = DirectControl.HandleInteractionWithSettlement(__instance.MapEntity.Settlement);

        return !handled;
    }

    public static bool MobileParty_SetMoveGoToPoint()
    {
        var handled = DirectControl.HandleInteractionWithoutTarget();

        return !handled;
    }
#endif
}