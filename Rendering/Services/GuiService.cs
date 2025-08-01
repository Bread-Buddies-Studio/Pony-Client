using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Core.Services;
using Rendering.Entities;
using Rendering.Services;
using Rendering.Visible;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
            Width = VisualRoot.ClientSize.Width;
            Height = VisualRoot.ClientSize.Height;
        }
    }
    public override void Render(DrawingContext context)
    {
        // Pre-Conditions //
        base.Render(context);
        // Invoke //
        RunService.InvokeRender(this, context);
        // Get Camera //
        LocalPlayer? localPlayer = WorldService.LocalPlayer;
        Camera? localCamera = WorldService.LocalPlayer?.AttachedCamera;
        // Conditions //
        if (localPlayer is null)
            return;
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
            // Render //
            renderable.OnRender(this, context);
        }
    }
}