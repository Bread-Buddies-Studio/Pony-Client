using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.Templates;
using Rendering.Entities;
using Rendering.Visible;
using Core.Extensions;
using Rendering.Views;
using Core.Services;
using Network.Services;
using LiteNetLib;
using Network.Enums;
using Tmds.DBus.Protocol;
using Network.Extensions;
using LiteNetLib.Utils;

namespace Rendering.Services;

[method: SetsRequiredMembers]
public class ChatEventArgs(string Message) : EventArgs
{
    // Data //
    public required string Message = Message;
}

public static class ChatService
{
    // Constructors //
    static ChatService()
    {
        NetworkService.NetworkReceive += OnNetworkReceive;
    }
    // Data //
    public static List<string> Log
    {
        get => field;

        private set => field = value;
    } = [];
    // Events //
    public static event EventHandler<ChatEventArgs>? Chatted;
    // Base Methods //
    public static void AddLog(Entity speaker, string message)
    {
        Debug.WriteLine(speaker.Name);
        Log.Add($"{speaker.Name}: {message}");
        // Create Text Bubble //
        _ = new Speech(speaker, message);
        // Fire Event //
        Chatted?.Invoke(speaker, new ChatEventArgs(message));
    }
    // Network Methods //
    private static void OnNetworkReceive(object? sender, (NetPeer client, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod) eventData)
    {
        // Pre-Conditions //
        if (eventData.reader.EndOfData)
            return;
        // Extract Type //
        PacketType packetType = (PacketType)eventData.reader.PeekByte();
        // Conditions //
        if (packetType is not PacketType.Message)
            return;
        // Skip //
        eventData.reader.SkipByte();
        // Debug //
        Debug.WriteLine("Received Message!");
        // Continue Extraction //
        string accountData = eventData.reader.GetString();
        string chatMessage = eventData.reader.GetString();
        // Find Speaker //
        Player speaker = Player.FindFromAccountData(accountData);
        // Activate //
        speaker.Speak(chatMessage);
    }
}

[method: SetsRequiredMembers]
public class Speech(Entity Speaker, string Message) : OverheadText(Speaker, Message)
{
    // Public Data //
    public double FadeTime = 5_000;
    // Overriden Data //
    public override bool IsAlwaysOnTop => true;
    // Private Data //
    private long lifeTime = 0;
    // Event Methods //
    public override void OnUpdate(object? sender, UpdateEventArgs eventData)
    {
        // Calculate Offset //
        base.OnUpdate(sender, eventData);
        // Extract Data //
        long deltaTime = eventData.DeltaTime;
        // Update Time //
        lifeTime += deltaTime;
        // Calculate //
        double alpha = (1 - lifeTime / FadeTime).Clamp01();
        // Update Opacity //
        Opacity = alpha;
        // Conditions //
        if (Opacity is 0)
            Dispose(); // Delete //
    }
    // Inherited Methods //
    public override void Dispose()
    {
        base.Dispose();
    }
}