using Bannerlord.ButterLib.HotKeys;
using TaleWorlds.InputSystem;
using HotKeyManager = Bannerlord.ButterLib.HotKeys.HotKeyManager;

namespace Bannerlord.PartyDirectControl;

public class Hotkeys
{
    public static DirectControlShowPanelKey ShowPanelKey = null!;
    public static DirectControlModifierKey ModifierKey = null!;

    public static void Register(string ns)
    {
        var hotKeyManager = HotKeyManager.Create(ns)!;

        ShowPanelKey = hotKeyManager.Add<DirectControlShowPanelKey>();
        ModifierKey = hotKeyManager.Add<DirectControlModifierKey>();

        hotKeyManager.Build();
    }
}

public class DirectControlShowPanelKey : HotKeyBase
{
    protected override string DisplayName { get; }
    protected override string Description { get; }
    protected override InputKey DefaultKey { get; }
    protected override string Category { get; }

    public DirectControlShowPanelKey()
        : base(nameof(DirectControlShowPanelKey))
    {
        DisplayName = "Party Direct Control - Display panel";
        Description = "Opens the Party Direct Control panel";
        DefaultKey = InputKey.X;
        Category = HotKeyManager.Categories[HotKeyCategory.CampaignMap];
    }
}

public class DirectControlModifierKey : HotKeyBase
{
    protected override string DisplayName { get; }
    protected override string Description { get; }
    protected override InputKey DefaultKey { get; }
    protected override string Category { get; }

    public DirectControlModifierKey()
        : base(nameof(DirectControlModifierKey))
    {
        DisplayName = "Party Direct Control - Modifier";
        Description = "The key to use in order to command selected parties.";
        DefaultKey = InputKey.LeftAlt;
        Category = HotKeyManager.Categories[HotKeyCategory.CampaignMap];
    }

    public bool IsHeld { get; private set; }

    protected override void OnPressed()
    {
        IsHeld = true;
    }

    protected override void OnReleased()
    {
        IsHeld = false;
    }
}