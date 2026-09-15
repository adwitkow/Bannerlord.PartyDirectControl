#if !LOWER_THAN_1_3
using Bannerlord.PartyDirectControl.Domain;
using HarmonyLib;
using HarmonyLib.PatchBuilder;
using NavalDLC.View.Map.Visuals;

namespace Bannerlord.PartyDirectControl.Patches;

public partial class DirectCommandPatches
{
    public static void ApplyForNavalDlc(Harmony harmony)
    {
        harmony.Patch<NavalMobilePartyVisual>()
            .Method(x => x.OnMapClick(default))
                .Prefix(MobilePartyVisual_OnMapClick);
    }

    public static bool NavalMobilePartyVisual_OnMapClick(ref NavalMobilePartyVisual __instance, ref bool __result)
    {
        var handled = DirectControl.HandleInteractionWithParty(__instance.MapEntity.MobileParty);

        return !handled;
    }
}
#endif