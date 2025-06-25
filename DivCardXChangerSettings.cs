using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using System.Windows.Forms;

namespace DivCardXChanger;

public class DivCardXChangerSettings : ISettings
{
    //Mandatory setting to allow enabling/disabling your plugin
    public ToggleNode Enable { get; set; } = new ToggleNode(false);

    public RangeNode<int> WaitClickMS { get; set; } = new RangeNode<int>(500, 0, 500);

    public HotkeyNode ToggleHotkey { get; set; } = new HotkeyNode(Keys.F2);
    public RangeNode<int> inventoryIndexHack { get; set; } = new RangeNode<int>(83, 1, 100);

    //Put all your settings here if you can.
    //There's a bunch of ready-made setting nodes,
    //nested menu support and even custom callbacks are supported.
    //If you want to override DrawSettings instead, you better have a very good reason.
}