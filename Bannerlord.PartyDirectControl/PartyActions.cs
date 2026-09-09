using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace Bannerlord.PartyDirectControl;

public static class PartyActions
{
    public static void VisitSettlement(MobileParty party, Settlement target)
    {
        var navigationData = CalculateNavigationData(party, target);

        party.Ai.SetDoNotMakeNewDecisions(true);

#if LOWER_THAN_1_3
        SetPartyAiAction.GetActionForVisitingSettlement(party, target);
#else
        SetPartyAiAction.GetActionForVisitingSettlement(
            party,
            target,
            navigationData.BestNavigationType,
            navigationData.IsFromPort,
            navigationData.IsTargetingPort);
#endif
    }

    public static void DefendSettlement(MobileParty party, Settlement target)
    {
        var navigationData = CalculateNavigationData(party, target);

        party.Ai.SetDoNotMakeNewDecisions(true);

#if LOWER_THAN_1_3
        SetPartyAiAction.GetActionForDefendingSettlement(party, target);
#else
        SetPartyAiAction.GetActionForDefendingSettlement(
            party,
            target,
            navigationData.BestNavigationType,
            navigationData.IsFromPort,
            navigationData.IsTargetingPort);
#endif
    }

    public static void BesiegeSettlement(MobileParty party, Settlement target)
    {
        var navigationData = CalculateNavigationData(party, target);

        party.Ai.SetDoNotMakeNewDecisions(true);

#if LOWER_THAN_1_3
        SetPartyAiAction.GetActionForBesiegingSettlement(party, target);
#else
        SetPartyAiAction.GetActionForBesiegingSettlement(
            party,
            target,
            navigationData.BestNavigationType,
            navigationData.IsFromPort);
#endif
    }

    public static void RaidVillage(MobileParty party, Settlement target)
    {
        var navigationData = CalculateNavigationData(party, target);

        party.Ai.SetDoNotMakeNewDecisions(true);

#if LOWER_THAN_1_3
        SetPartyAiAction.GetActionForRaidingSettlement(party, target);
#elif LOWER_THAN_1_4
        SetPartyAiAction.GetActionForRaidingSettlement(
            party,
            target,
            navigationData.BestNavigationType,
            navigationData.IsFromPort);
#else
        SetPartyAiAction.GetActionForRaidingSettlement(
            party,
            target,
            navigationData.BestNavigationType,
            navigationData.IsFromPort,
            navigationData.IsTargetingPort);
#endif
    }

    public static void AttackParty(MobileParty party, MobileParty target)
    {
        var navigationData = CalculateNavigationData(party, target);

        party.Ai.SetDoNotMakeNewDecisions(true);

#if LOWER_THAN_1_3
        SetPartyAiAction.GetActionForEngagingParty(party, target);
#else
        SetPartyAiAction.GetActionForEngagingParty(
            party,
            target,
            navigationData.BestNavigationType,
            navigationData.IsFromPort);
#endif
    }

    public static void EscortParty(MobileParty party, MobileParty target)
    {
        var navigationData = CalculateNavigationData(party, target);

        party.Ai.SetDoNotMakeNewDecisions(true);

#if LOWER_THAN_1_3
        SetPartyAiAction.GetActionForEscortingParty(party, target);
#else
        SetPartyAiAction.GetActionForEscortingParty(
            party,
            target,
            navigationData.BestNavigationType,
            navigationData.IsFromPort,
            navigationData.IsTargetingPort);
#endif
    }

#if !LOWER_THAN_1_3
    private static NavigationData CalculateNavigationData(MobileParty party, MobileParty targetParty)
    {
        MobileParty.NavigationType navigationType = MobileParty.NavigationType.None;
        bool isTargetingPort = false;
        bool isFromPort = false;

        if (targetParty.CurrentSettlement is not null)
        {
            return CalculateNavigationData(party, targetParty.CurrentSettlement);
        }

        Helpers.AiHelper.GetBestNavigationTypeAndDistanceOfMobilePartyForMobileParty(
            party,
            targetParty,
            out navigationType,
            out _);

        return new NavigationData()
        {
            BestNavigationType = navigationType,
            IsFromPort = isFromPort,
            IsTargetingPort = isTargetingPort,
        };
    }

    private static NavigationData CalculateNavigationData(MobileParty party, Settlement targetSettlement)
    {
        bool isTargetingPort = targetSettlement.HasPort && party.IsCurrentlyAtSea;

        Helpers.AiHelper.GetBestNavigationTypeAndAdjustedDistanceOfSettlementForMobileParty(
            party,
            targetSettlement,
            isTargetingPort: isTargetingPort,
            out var navigationType,
            out var bestDistance,
            out var isFromPort);

        return new NavigationData()
        {
            BestNavigationType = navigationType,
            IsFromPort = isFromPort,
            IsTargetingPort = isTargetingPort,
        };
    }

    private record struct NavigationData(
        MobileParty.NavigationType BestNavigationType,
        bool IsFromPort,
        bool IsTargetingPort);
#else
    // TODO: Make it so stubs aren't necessary
    private static object? CalculateNavigationData(MobileParty party, MobileParty targetParty)
    {
        return null;
    }

    private static object? CalculateNavigationData(MobileParty party, Settlement targetSettlement)
    {
        return null;
    }
#endif
}
