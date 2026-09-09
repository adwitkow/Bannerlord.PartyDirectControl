using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Bannerlord.PartyDirectControl.GameModels;

public class DirectControlArmyManagementCalculationModel : ArmyManagementCalculationModel
{
    public override bool CanLordCreateArmy(MobileParty leaderParty, out MBList<MobileParty> possibleArmyMembers)
    {
        var result = BaseModel.CanLordCreateArmy(leaderParty, out possibleArmyMembers);

        RemoveForbiddenPartiesFromArmyCall(leaderParty, possibleArmyMembers);

        return result;
    }

    private static void RemoveForbiddenPartiesFromArmyCall(MobileParty thinkingParty, MBList<MobileParty>? partiesToCallToArmy)
    {
        if (partiesToCallToArmy is null)
        {
            return;
        }

        for (int index = partiesToCallToArmy.Count - 1; index >= 0; index--)
        {
            MobileParty candidate = partiesToCallToArmy[index];

            if (SubModule.DirectControlBehavior.IsPartyUnderControl(candidate))
            {
                partiesToCallToArmy.RemoveAt(index);
            }
        }
    }

    #region Pass-through overrides
    public override float AIMobilePartySizeRatioToCallToArmy => BaseModel.AIMobilePartySizeRatioToCallToArmy;

    public override float PlayerMobilePartySizeRatioToCallToArmy => BaseModel.PlayerMobilePartySizeRatioToCallToArmy;

    public override float MinimumNeededFoodInDaysToCallToArmy => BaseModel.MinimumNeededFoodInDaysToCallToArmy;

    public override float MaximumDistanceToCallToArmy => BaseModel.MaximumDistanceToCallToArmy;

    public override int InfluenceValuePerGold => BaseModel.InfluenceValuePerGold;

    public override int AverageCallToArmyCost => BaseModel.AverageCallToArmyCost;

    public override int CohesionThresholdForDispersion => BaseModel.CohesionThresholdForDispersion;

    public override float MaximumWaitTime => BaseModel.MaximumWaitTime;

    public override ExplainedNumber CalculateDailyCohesionChange(Army army, bool includeDescriptions = false)
    {
        return BaseModel.CalculateDailyCohesionChange(army, includeDescriptions);
    }

    public override int CalculateNewCohesion(Army army, PartyBase newParty, int calculatedCohesion, int sign)
    {
        return BaseModel.CalculateNewCohesion(army, newParty, calculatedCohesion, sign);
    }

    public override int CalculatePartyInfluenceCost(MobileParty armyLeaderParty, MobileParty party)
    {
        return BaseModel.CalculatePartyInfluenceCost(armyLeaderParty, party);
    }

    public override int CalculateTotalInfluenceCost(Army army, float percentage)
    {
        return BaseModel.CalculateTotalInfluenceCost(army, percentage);
    }

    public override bool CanPlayerCreateArmy(out TextObject disabledReason)
    {
        return BaseModel.CanPlayerCreateArmy(out disabledReason);
    }

    public override bool CheckPartyEligibility(MobileParty party, out TextObject explanation)
    {
        return BaseModel.CheckPartyEligibility(party, out explanation);
    }

    public override float DailyBeingAtArmyInfluenceAward(MobileParty armyMemberParty)
    {
        return BaseModel.DailyBeingAtArmyInfluenceAward(armyMemberParty);
    }

    public override int GetCohesionBoostInfluenceCost(Army army, int percentageToBoost = 100)
    {
        return BaseModel.GetCohesionBoostInfluenceCost(army, percentageToBoost);
    }

    public override int GetPartyRelation(Hero hero)
    {
        return BaseModel.GetPartyRelation(hero);
    }

    public override float GetPartySizeScore(MobileParty party)
    {
        return BaseModel.GetPartySizeScore(party);
    }
    #endregion
}
