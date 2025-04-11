// Imports //
using Avalonia.Media;
using Avalonia.Threading;
using Pony_Client.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Directory //
namespace Pony_Client.Services;
// Event Args //
public class UpdateEventArgs : EventArgs
{
    // Private Data //
    private static long lastInvoke = GetCurrentMillis();
    // Public Data //
    public readonly long DeltaTime;
    // Constructor //
    public UpdateEventArgs() : base()
    {
        // Data //
        long currentMillis = GetCurrentMillis();
        // Settings //
        DeltaTime = currentMillis - lastInvoke;
        // Reset //
        lastInvoke = GetCurrentMillis();
    }
    // Private Methods //
    static long GetCurrentMillis() => DateTimeOffset.Now.ToUnixTimeMilliseconds();
}
// Class //
public static class RunService
{
    // Private Data //
    private static readonly DispatcherTimer Timer = new()
    {
        Interval = TimeSpan.FromMilliseconds(1000f / FPS),
        IsEnabled = true
    };
    // Public Events //
    public static event EventHandler<UpdateEventArgs>? Update;
    public static event EventHandler<DrawingContext>? Render;
    // Constructor //
    static RunService()
    {
        Timer.Tick += OnUpdate;
    }
    // Private Methods //
    private static void OnUpdate(object? sender, EventArgs _) => Update?.Invoke(Timer, new UpdateEventArgs());
    // Public Methods //
    public static void InvokeRender(GuiService guiService, DrawingContext context) => Render?.Invoke(guiService, context);
    public static async Task UntilNextRender()
    {
        // Event Resetter to wait 1 frame //
        TaskCompletionSource<bool> taskAwaiter = new();
        // Methods //
        void OnRender(object? sender, DrawingContext context) => taskAwaiter.SetResult(true);
        // Connections //
        Render += OnRender;
        // Await Frame //
        await taskAwaiter.Task;
        // Disconnect //
        Render -= OnRender;
    }
    public static async Task UntilNextUpdate()
    {
        // Event Resetter to wait 1 frame //
        TaskCompletionSource<bool> taskAwaiter = new();
        // Methods //
        void OnUpdate(object? sender, UpdateEventArgs _) => taskAwaiter.SetResult(true);
        // Connections //
        Update += OnUpdate;
        // Await Frame //
        await taskAwaiter.Task;
        // Disconnect //
        Update -= OnUpdate;
    }
}
