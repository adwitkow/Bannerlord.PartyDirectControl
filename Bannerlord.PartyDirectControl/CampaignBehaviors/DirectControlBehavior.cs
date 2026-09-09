using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace Bannerlord.PartyDirectControl.CampaignBehaviors;

public class DirectControlBehavior : CampaignBehaviorBase
{
    private List<MobileParty> _partiesUnderControl = new();

    public override void RegisterEvents()
    {
        CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, OnHourlyTickParty);
        CampaignEvents.OnPartyJoinedArmyEvent.AddNonSerializedListener(this, OnPartyJoinedArmy);
        CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, OnMobilePartyCreated);
        CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, OnMobilePartyDestroyed);
    }

    public override void SyncData(IDataStore dataStore)
    {
        dataStore.SyncData(nameof(_partiesUnderControl), ref _partiesUnderControl);
    }

    public void RepopulatePartiesUnderControl(IEnumerable<MobileParty> parties)
    {
        _partiesUnderControl.Clear();
        _partiesUnderControl.AddRange(parties);

        ApplyToPartiesUnderControl(party => PartyActions.EscortParty(party, MobileParty.MainParty));
    }

    public void ApplyToPartiesUnderControl(Action<MobileParty> action)
    {
        _partiesUnderControl.ForEach(party => action(party));
    }

    public bool IsPartyUnderControl(MobileParty party)
    {
        return _partiesUnderControl.Contains(party);
    }

    private void OnHourlyTickParty(MobileParty party)
    {
        if (!_partiesUnderControl.Contains(party))
        {
            return;
        }

        if (party.DefaultBehavior == AiBehavior.Hold)
        {
            PartyActions.EscortParty(party, MobileParty.MainParty);
        }
    }

    private void OnPartyJoinedArmy(MobileParty party)
    {
        RemoveFromControl(party);
    }

    private void OnMobilePartyCreated(MobileParty party)
    {
        RemoveFromControl(party);
    }

    private void OnMobilePartyDestroyed(MobileParty destroyedParty, PartyBase destroyerParty)
    {
        RemoveFromControl(destroyedParty);
    }

    private void RemoveFromControl(MobileParty party)
    {
        party.Ai.RethinkAtNextHourlyTick = true;
        party.Ai.SetDoNotMakeNewDecisions(false);

        _partiesUnderControl.Remove(party);
    }
}