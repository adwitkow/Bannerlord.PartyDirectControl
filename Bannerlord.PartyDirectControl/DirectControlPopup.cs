using Bannerlord.PartyDirectControl.CampaignBehaviors;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
#if !LOWER_THAN_1_3
using TaleWorlds.Core.ImageIdentifiers;
#endif
using TaleWorlds.Localization;

namespace Bannerlord.PartyDirectControl;

internal class DirectControlPopup
{
    private static readonly TextObject TitleText = new("{=PAIFHytp3D7}Choose which parties to directly command");
    private static readonly TextObject DescriptionText = new("{=PAIRzSgh49H}Parties must be manageable and in visual range to appear here.");
    
    private readonly DirectControlBehavior _directControlBehavior;

    private bool IsPopupOpen = false;

    public DirectControlPopup(DirectControlBehavior directControlBehavior)
    {
        _directControlBehavior = directControlBehavior;
    }

    public void OpenPopup()
    {
        if (IsPopupOpen)
        {
            return;
        }

        CampaignTimeControlMode mode = Campaign.Current.TimeControlMode;
        Campaign.Current.TimeControlMode = CampaignTimeControlMode.FastForwardStop;

        string title = TitleText.ToString();
        string desc = DescriptionText.ToString();

        List<InquiryElement> inquiryElements = MobileParty.AllLordParties
            .Where(ShouldBeIncludedInPopup)
            .OrderByDescending(m => m.ActualClan.Equals(Clan.PlayerClan))
            .ThenBy(m => m.Name?.ToString())
            .Select(ConvertToInquiryElement)
            .ToList();

        MBInformationManager.ShowMultiSelectionInquiry(new(
            title,
            desc,
            inquiryElements,
            isExitShown: true,
            minSelectableOptionCount: 0,
            maxSelectableOptionCount: inquiryElements.Count,
            GameTexts.FindText("str_done").ToString(),
            GameTexts.FindText("str_cancel").ToString(),
            affirmativeAction: results => ConfirmSelection(results, mode),
            negativeAction: _ => ClosePopup(mode),
            isSeachAvailable: true)
        );

        IsPopupOpen = true;
    }

    private void ConfirmSelection(List<InquiryElement> results, CampaignTimeControlMode mode)
    {
        var parties = results
            .Select(e => e.Identifier)
            .OfType<MobileParty>();

        _directControlBehavior.RepopulatePartiesUnderControl(parties);

        ClosePopup(mode);
    }

    private void ClosePopup(CampaignTimeControlMode mode)
    {
        IsPopupOpen = false;
        Campaign.Current.TimeControlMode = mode;
    }

    private bool ShouldBeIncludedInPopup(MobileParty mobileParty)
    {
        var mainHero = Hero.MainHero;

        if (mobileParty.LeaderHero == mainHero)
        {
            return false;
        }

        bool belongsToPlayer = mainHero.IsKingdomLeader
            ? mobileParty.MapFaction == mainHero.MapFaction
            : mobileParty.ActualClan == mainHero.Clan;

        return belongsToPlayer
            && IsWithinSeeingRange(mobileParty);
    }

    private static InquiryElement ConvertToInquiryElement(MobileParty mobileParty)
    {
        var characterCode = CharacterCode.CreateFrom(mobileParty.LeaderHero?.CharacterObject);
#if LOWER_THAN_1_3
        var imageIdentifier = new ImageIdentifier(characterCode);
#else
        var imageIdentifier = new CharacterImageIdentifier(characterCode);
#endif

        return new InquiryElement(
            identifier: mobileParty,
            title: mobileParty.Name.ToString(),
            imageIdentifier: imageIdentifier);
    }

    private static bool IsWithinSeeingRange(MobileParty mobileParty)
    {
        var distance = mobileParty.GetPosition2D.Distance(MobileParty.MainParty.GetPosition2D);
        return distance <= MobileParty.MainParty.SeeingRange;
    }
}
