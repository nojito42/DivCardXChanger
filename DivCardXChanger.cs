using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets.Components;
using SharpDX;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Stack = ExileCore.PoEMemory.Components.Stack;
using Vector2 = System.Numerics.Vector2;

namespace DivCardXChanger;



public class DivCardXChanger : BaseSettingsPlugin<DivCardXChangerSettings>
{
    public CardTradeWindow CardTradeWindow { get; private set; }
    public InventoryElement InventoryWindow { get; private set; }

    private bool canPress = true;
    private Coroutine cardTradeCoroutine;

    public override bool Initialise()
    {
        return true;
    }

    public override void AreaChange(AreaInstance area)
    {
        canPress = true;
    }

    public override Job Tick()
    {
        CardTradeWindow ??= GameController.IngameState.IngameUi.CardTradeWindow;
        InventoryWindow ??= GameController.IngameState.IngameUi.InventoryPanel;

        if (!canPress || InventoryWindow == null || CardTradeWindow == null)
            return null;

        if (Settings.ToggleHotkey.PressedOnce())
        {
            canPress = false;
            cardTradeCoroutine = new Coroutine(ProcessDivinationCards(), this, "DivCardXChangerCoroutine", autoStart: true);
            Core.ParallelRunner.Run(cardTradeCoroutine);
        }

        return null;
    }

    public override void Render()
    {
        if (!Settings.Enable || MenuWindow.IsOpened)
            return;

        if (CardTradeWindow?.IsVisibleLocal == true && InventoryWindow.IsVisibleLocal)
        {
            Graphics.DrawFrame(CardTradeWindow.GetClientRect(), Color.Green, 4);
            Graphics.DrawFrame(InventoryWindow.GetClientRect(), Color.Green, 4);
        }      
    }

    public IEnumerator ProcessDivinationCards()
    {
        if (!Settings.Enable || CardTradeWindow == null || InventoryWindow == null)
        {
            canPress = true;
            yield break;
        }

        var divTab = InventoryWindow[3][Settings.inventoryIndexHack];
        if (divTab == null || divTab.ChildCount <= 0)
        {
            LogError("No divination cards found.");
            canPress = true;
            yield break;
        }

        Input.KeyDown(Keys.LControlKey);
        Thread.Sleep(Settings.WaitClickMS);

        for (int i = 0; i < divTab.Children.Count; i++)
        {
            var divCard = divTab[i];
            if (divCard?.Entity == null || divCard.Entity.Metadata == null)
                continue;

            var stack = divCard.Entity.GetComponent<Stack>();
            if (stack == null || !stack.FullStack)
                continue;

            if (!divCard.Entity.Metadata.StartsWith("Metadata/Items/DivinationCards"))
                continue;

            var rect = divCard.GetClientRect();
            var center = divCard.Center;
            Graphics.DrawFrame(rect, Color.Red, 2);

            Input.SetCursorPos(new Vector2(center.X,center.Y));
            Thread.Sleep(Settings.WaitClickMS);
            Input.Click(MouseButtons.Left);
            Thread.Sleep(Settings.WaitClickMS);

            var tradeButton = CardTradeWindow.TradeButton;
            if (tradeButton != null && tradeButton.IsVisibleLocal && tradeButton.IsActive)
            {
                Input.SetCursorPos(tradeButton.GetClientRect().Center.ToVector2Num());
                Thread.Sleep(Settings.WaitClickMS);
                Input.Click(MouseButtons.Left);
                Thread.Sleep(Settings.WaitClickMS);

                var dump = CardTradeWindow[5][1];
                if (dump?.IsVisibleLocal == true)
                {
                    Input.SetCursorPos(dump.GetClientRect().Center.ToVector2Num());
                    Thread.Sleep(Settings.WaitClickMS);
                    Input.Click(MouseButtons.Left);
                    Thread.Sleep(Settings.WaitClickMS);
                }
            }
        }

        Input.KeyUp(Keys.LControlKey);
        canPress = true;
        yield break;
    }

    public override void EntityAdded(Entity entity) { }
}
