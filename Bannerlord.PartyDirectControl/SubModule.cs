using Bannerlord.PartyDirectControl.CampaignBehaviors;
using Bannerlord.PartyDirectControl.GameModels;
using Bannerlord.PartyDirectControl.Patches;
using HarmonyLib;
#if LOWER_THAN_1_3
using System.Linq;
#endif
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.PartyDirectControl;

public class SubModule : MBSubModuleBase
{
    private static readonly string Namespace = typeof(SubModule).Namespace;

    public static DirectControlBehavior DirectControlBehavior = null!;

    private static DirectControlPopup _directControlPopup = null!;
    private static bool HotkeysRegistered = false;

    protected override void OnSubModuleLoad()
    {
        base.OnSubModuleLoad();

        var harmony = new Harmony(Namespace);

        DirectCommandPatches.Apply(harmony);
    }

    protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
    {
        base.OnGameStart(game, gameStarterObject);

        if (gameStarterObject is not CampaignGameStarter campaignGameStarter
            || game.GameType is not Campaign)
        {
            return;
        }

        if (!HotkeysRegistered)
        {
            Hotkeys.Register(Namespace);
            Hotkeys.ShowPanelKey.OnPressedEvent += OpenDirectControlPopup;
            HotkeysRegistered = true;
        }

        DirectControlBehavior = new DirectControlBehavior();
        campaignGameStarter.AddBehavior(DirectControlBehavior);

#if LOWER_THAN_1_3
        var baseModel = gameStarterObject.Models
            .OfType<ArmyManagementCalculationModel>()
            .LastOrDefault();
        gameStarterObject.AddModel(new DirectControlArmyManagementCalculationModel(baseModel));
#else
        campaignGameStarter.AddModel<ArmyManagementCalculationModel>(new DirectControlArmyManagementCalculationModel());
#endif

        _directControlPopup = new DirectControlPopup(DirectControlBehavior);
    }

    private void OpenDirectControlPopup()
    {
        var activeState = Game.Current?.GameStateManager?.ActiveState;

        if (activeState == null
            || activeState is not MapState
            || activeState.IsMenuState
            || activeState is MissionState
            || Mission.Current != null)
        {
            return;
        }

        _directControlPopup.OpenPopup();
    }
}