using Avalonia.Controls;
using Avalonia.Interactivity;
using LiteNetLib;
using LiteNetLib.Utils;
using Pony_Client.Services;
using Pony_Client.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Pony_Client;
// API //
public enum ConnectionFail
{
    IncorrectPassword,
    AccountNotFound,
    InvalidTagLine,
    // Sign Up //
    AccountAlreadyExists,

}
public enum ConnectionType
{
    LogIn,
    SignUp
}
public enum API
{
    LogIn,
    SignUp,
    Message
}
// Class //
public class Client : INetEventListener
{
    public readonly NetManager client;
    public readonly NetPeer server;
    public readonly EventBasedNetListener listener;
    // Data //
    public ConnectionFail? failReason;
    // Constructor //
    /// <summary>
    /// Create a new client
    /// </summary>
    /// <param name="connectionData">
    /// Tagline, Username, and Password:
    /// 000|UserName|PassWord
    /// </param>
    /// <param name="connectionType">
    /// <see cref="API.LogIn"/> to log in, 
    /// <see cref="API.SignUp"/> to sign up.
    /// </param>
    public Client(string connectionData, API connectionType)
    {
        // Initialize Client //
        listener = new();
        client = new NetManager(this);
        client.Start();
        // Data //
        NetDataWriter writer = new();
        writer.Put((int)connectionType);
        writer.Put(connectionData);
        // Connect //
        server = client.Connect("localhost", PortNum, writer);
    }
    #region Network Methods
    #endregion
    #region Inherited Network Methods
    public void OnConnectionRequest(ConnectionRequest request)
    {
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        failReason = ConnectionFail.IncorrectPassword;
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        // Receive Data //
        API dataType = (API)reader.GetInt();
        
        switch(dataType)
        {
            case API.Message:
                string message = reader.GetString();
                //ChatService.AddLog(message);
                break;
        }
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
    }

    public void OnPeerConnected(NetPeer peer)
    {
        
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) // Got Kicked Out By Server //
    {
        // Conditions //
        if (peer != server)
            return;
        // Data Source //
        NetDataReader reader = disconnectInfo.AdditionalData;
        // Extract Data //
        if (reader.TryGetInt(out int failData))
            failReason = (ConnectionFail)failData;
    }
    #endregion
}