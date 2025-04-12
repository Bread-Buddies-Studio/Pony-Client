using Avalonia;
using Avalonia.Media;
using Pony_Client.Entities;
using Pony_Client.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pony_Client.Services;

public static class WorldService
{
    public static LocalPlayer? localPlayer;
    // Constructor //
    static WorldService()
    {
        for (int i = 1; i <= 5; i++)
        {
            Debug.WriteLine("Creating Structure!");
            _ = new Structure(new(500 * i, 0, 400, 400))
            { Layer = (byte)i };        }

        NPC testNPC = new(
                Name: "Fredrick",
                Position: new Point(200, 200),
                Size: new Size(200, 200)
            )
        { Layer = 1 };

        testNPC.Speak("Hello There");
    }
    // Event Methods //
    public static void OnRender(object? _guiService, (byte layer, DrawingContext context) graphics)
    {
        // Conditions //
        if (_guiService is null)
            return;
        // Main Window //
        GuiService window = (GuiService)_guiService;
    }
}

public class Structure : Renderable
{
    // Static Data //
    public readonly static List<Structure> Structures = [];
    // Render Data //
    public readonly List<DrawingContext.PushedState> Matrixes = [];
    // Constructors //
    [SetsRequiredMembers]
    public Structure(Rect Hitbox) : base(Hitbox)
    {
        // Settings //
        this.Hitbox = Hitbox;
        // Add Reference //
        Structures.Add(this);
        // Connections //
        RunService.Update += OnUpdate;
    }
    [SetsRequiredMembers]
    public Structure(Point Position, Size Size) : this(new Rect())
    {
        this.Position = Position;
        this.Size = Size;
    }
    // Event Methods //
    public void OnUpdate(object? sender, UpdateEventArgs eventData) { }
    public override void OnRender(GuiService guiService, DrawingContext context)
    {
        // Data //
        LocalPlayer? localPlayer = WorldService.localPlayer;
        // Conditions //
        if (localPlayer is null)
            return;
        if (localPlayer.AttachedCamera is null)
            return;
        // Camera Transform //
        PushTransform(context, localPlayer.AttachedCamera.OffsetMatrix);
        // Draw //
        context.DrawRectangle(Brushes.White, null, Hitbox);
        // Reset Matrix //
        PopMatrix();
    }
    // Disposal Methods //
    public override void Delete()
    {
        Structures.Remove(this);
        base.Delete();
    }
}