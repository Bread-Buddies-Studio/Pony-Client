using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Core.Extensions;
using Core.Services;
using Rendering.Entities;
using Rendering.Services;
using Rendering.Visible;

namespace Rendering.Views;
// Service //
public partial class GuiService : UserControl
{
    public static GuiService? Window { get; private set; }
    // Constructor //
    public GuiService()
    {
        // Initialize //
        InitializeComponent();
        // Connections //
        RunService.Update += OnUpdate;
        ChatService.Chatted += OnChatted;
        InputService.KeyDown += OnKeyDown;
        // Reference //
        Window = this;
        // Fixes Pixel Art //
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.None);
    }
    // Static Methods //
    public static void ClearFocus()
    {
        // Conditions //
        if (Window is null)
            return;
        // Settings //
        Window.Focusable = true;
        Window.Focus();
        Window.Focusable = false;
    }
    public static WriteableBitmap CropBitmap(Bitmap source, PixelRect cropRect) => source.CopyPixels(cropRect);
    // Event Methods //
    public async void OnKeyDown(object? sender, KeyEventArgs eventData)
    {
        // Conditions //
        if (WorldService.LocalPlayer is null)
            return;
        // Checks //
        switch(eventData.Key)
        {
            case Key.Enter:
                // Conditions //
                if (!ChatInput.IsFocused)
                    break;
                if (string.IsNullOrEmpty(ChatInput.Text))
                    break;
                // Success //
                WorldService.LocalPlayer.Speak(ChatInput.Text);
                ChatInput.Clear();
                ClearFocus();
                break;
            case Key.OemQuestion:
                // Conditions //
                if (ChatInput.IsFocused)
                    break;
                // Focus Text Input //
                ChatInput.Focus();
                // Delay //
                await Task.Yield();
                // Delete Slash //
                ChatInput.Clear();
                break;
            case Key.Escape:
                ClearFocus();
                break;
            default:
                break;
        }
    }
    public void OnChatted(object? _speaker, ChatEventArgs eventData)
    {
        // Get Text //
        string Log = string.Empty;

        foreach (string message in ChatService.Log)
            Log += $"{message}\n";
        // Show Text //
        ChatLog.Text = Log;
        Focus();
        //TopLevel.GetTopLevel(this)?.FocusManager?.ClearFocus();
    }
    public void OnUpdate(object? _, UpdateEventArgs eventData)
    {
        // Checks //
        if (VisualRoot is not null)
        {   // Update Size //
            Width = VisualRoot.Bounds.Width;
            Height = VisualRoot.Bounds.Height;
        }
    }
    public override void Render(DrawingContext context)
    {
        // Pre-Conditions //
        base.Render(context);
        // Invoke //
        RunService.InvokeRender(this, context);
        // Get Camera //
        Camera? localCamera = WorldService.CurrentCamera;
        // Conditions //
        if (localCamera is null)
            return;
        // Invoke Events //
        foreach (var renderable in Renderable.Renderables)
        {
            // Conditions //
            if (!renderable.AlwaysShows && !localCamera.IsWithinSight(renderable))
                continue;
            if (!renderable.IsVisible)
                continue;
            // Debug //
            //Debug.WriteLine($"Rendering at {renderable.Position} on layer {renderable.Layer}");
            // Render //
            renderable.OnRender(this, context);
        }
    }
}