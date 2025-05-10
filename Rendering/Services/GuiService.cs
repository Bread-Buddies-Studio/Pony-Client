using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Core.Services;
using Rendering.Services;
using Rendering.Visible;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Rendering.Views;
// Service //
public partial class GuiService : UserControl
{
    public static GuiService? Window;
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
    }
    // Static Methods //
    public static Bitmap LoadFromResource(Uri resourceUri)
    {
        return new Bitmap(AssetLoader.Open(resourceUri));
    }
    public static async Task<Bitmap?> LoadFromWeb(Uri url)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            return new Bitmap(new MemoryStream(data));
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
            return null;
        }
    }
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
                break;
            case Key.OemQuestion:
                ChatInput.Focus();
                await RunService.UntilNextUpdate();
                ChatInput.Clear();
                break;
            case Key.Escape:
                TopLevel.GetTopLevel(this)?.FocusManager?.ClearFocus();
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
        TopLevel.GetTopLevel(this)?.FocusManager?.ClearFocus();
    }
    public void OnUpdate(object? _, UpdateEventArgs eventData)
    {
        // Checks //
        if (VisualRoot is not null)
        {   // Update Size //
            Width = VisualRoot.ClientSize.Width;
            Height = VisualRoot.ClientSize.Height;
        }
    }
    public override void Render(DrawingContext context)
    {
        // Conditions //
        base.Render(context);
        // Invoke //
        RunService.InvokeRender(this, context);
        // Invoke Events //
        foreach (Renderable renderable in Renderable.Renderables)
        {
            if (WorldService.LocalPlayer is null)
                return;
            if (WorldService.LocalPlayer.AttachedCamera is null)
                return;
            // Conditions //
            if (!WorldService.LocalPlayer.AttachedCamera.IsWithinSight(renderable))
                continue;
            // Render //
            renderable.OnRender(this, context);
        }
    }
}
