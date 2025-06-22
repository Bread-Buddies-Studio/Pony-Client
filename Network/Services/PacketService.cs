using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using Network.Enums;

namespace Network.Services;

public static class PacketService
{
    // Constructor //
    static PacketService()
    {
        // Register Packets //
        // Register Packets //
    }
    // Base Methods //
    /// <summary>
    /// Sends a packet containing <paramref name="data"/> to the server if a connection is currently active.
    /// </summary>
    /// <param name="data">The data to send</param>
    /// <param name="deliveryMethod">The method the packet will be delivered, default is: <see cref="DeliveryMethod.ReliableSequenced"/></param>
    public static void SendPacket(
        PacketType packetType,
        INetSerializable data, 
        DeliveryMethod deliveryMethod = DeliveryMethod.ReliableSequenced)
    {
        // Conditions //
        if (CurrentClient is null)
            return;
        // Get Account Data //
        string? accountData = AccountService.GetAccountData();
        // Conditions  //
        if (accountData is null)
            return;
        // Initialize Writer //
        NetDataWriter writer = new();
        // Prepare Data //
        writer.Put((byte)packetType);
        writer.Put(data);
        // Send Data //
        CurrentClient.server.Send(writer, deliveryMethod);
    }

    /// <inheritdoc cref="SendPacket(PacketType, INetSerializable, DeliveryMethod)"/>
    public static void SendPacket(
        PacketType packetType,
        string data,
        DeliveryMethod deliveryMethod = DeliveryMethod.ReliableSequenced)
    {
        // Conditions //
        if (CurrentClient is null)
            return;
        // Initialize Writer //
        NetDataWriter writer = new();
        // Prepare Data //
        writer.Put((byte)packetType);
        writer.Put(data);
        // Send Data //
        CurrentClient.server.Send(writer, deliveryMethod);
    }
    
    /// <inheritdoc cref="SendPacket(PacketType, INetSerializable, DeliveryMethod)"/>
    public static void SendPacket(
        PacketType packetType,
        double[] data,
        DeliveryMethod deliveryMethod = DeliveryMethod.ReliableSequenced)
    {

        // Conditions //
        if (CurrentClient is null)
            return;

        // Initialize Writer //
        NetDataWriter writer = new();

        // Prepare Data //
        writer.Put((byte)packetType);
        writer.PutArray(data);

        // Send Data //
        CurrentClient.server.Send(writer, deliveryMethod);
    }
}

[method: SetsRequiredMembers]
public class PacketReader(NetDataReader Reader) : object()
{
    // Required Data //
    public required PacketType PacketType = (PacketType)Reader.PeekByte();
    public required NetDataReader Reader = Reader;
    // Data //
    public int Position = 1;
    // Extraction Methods //
    public byte GetByte()
    {
        Reader.SetPosition(Position++);
        return Reader.PeekByte();
        
    }
    public string GetString()
    {
        Reader.SetPosition(Position++);
        return Reader.PeekString();
    }
    public int GetInt()
    {
        Reader.SetPosition(Position++);
        return Reader.PeekInt();
    }
    public INetSerializable Get(INetSerializable container)
    {
        Reader.Get(() => container);
        return container;
    }
}