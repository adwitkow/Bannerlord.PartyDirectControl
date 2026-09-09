using Helpers;
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

        SetPartyAiAction.GetActionForVisitingSettlement(
            party,
            target,
            navigationData.BestNavigationType,
            navigationData.IsFromPort,
            navigationData.IsTargetingPort);
    }

    public static void DefendSettlement(MobileParty party, Settlement target)
    {
        var navigationData = CalculateNavigationData(party, target);

        party.Ai.SetDoNotMakeNewDecisions(true);

        SetPartyAiAction.GetActionForDefendingSettlement(
            party,
            target,
            navigationData.BestNavigationType,
            navigationData.IsFromPort,
            navigationData.IsTargetingPort);
    }

    public static void BesiegeSettlement(MobileParty party, Settlement target)
    {
        var navigationData = CalculateNavigationData(party, target);

        party.Ai.SetDoNotMakeNewDecisions(true);

        SetPartyAiAction.GetActionForBesiegingSettlement(
            party,
            target,
            navigationData.BestNavigationType,
            navigationData.IsFromPort);
    }

    public static void RaidVillage(MobileParty party, Settlement target)
    {
        var navigationData = CalculateNavigationData(party, target);

        party.Ai.SetDoNotMakeNewDecisions(true);

        SetPartyAiAction.GetActionForRaidingSettlement(
            party,
            target,
            navigationData.BestNavigationType,
            navigationData.IsFromPort,
            navigationData.IsTargetingPort);
    }

    public static void AttackParty(MobileParty party, MobileParty target)
    {
        var navigationData = CalculateNavigationData(party, target);

        party.Ai.SetDoNotMakeNewDecisions(true);

        SetPartyAiAction.GetActionForEngagingParty(
            party,
            target,
            navigationData.BestNavigationType,
            navigationData.IsFromPort);
    }

    public static void EscortParty(MobileParty party, MobileParty targetParty)
    {
        var navigationData = CalculateNavigationData(party, targetParty);

        party.Ai.SetDoNotMakeNewDecisions(true);

        SetPartyAiAction.GetActionForEscortingParty(
            party,
            targetParty,
            navigationData.BestNavigationType,
            navigationData.IsFromPort,
            navigationData.IsTargetingPort);
    }

    private static NavigationData CalculateNavigationData(MobileParty party, MobileParty targetParty)
    {
        MobileParty.NavigationType navigationType = MobileParty.NavigationType.None;
        bool isTargetingPort = false;
        bool isFromPort = false;

        if (targetParty.CurrentSettlement is not null)
        {
            return CalculateNavigationData(party, targetParty.CurrentSettlement);
        }

        AiHelper.GetBestNavigationTypeAndDistanceOfMobilePartyForMobileParty(
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

        AiHelper.GetBestNavigationTypeAndAdjustedDistanceOfSettlementForMobileParty(
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
}
