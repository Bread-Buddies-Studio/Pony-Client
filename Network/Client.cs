using LiteNetLib;
using LiteNetLib.Utils;
using Network.Enums;
using Network.Services;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Network;
// Class //
public class Client : EventBasedNetListener, INetEventListener
{
    public static Client? Singleton { get; private set; }
    public readonly NetManager client;
    public readonly NetPeer server;
    // Data //
    public bool Connected
    {
        get => server.ConnectionState is ConnectionState.Connected;
    }
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
        client = new NetManager(this);
        client.Start();
        // Data //
        NetDataWriter writer = new();
        writer.Put((int)connectionType);
        writer.Put(connectionData);
        // Connect //
        Debug.WriteLine("Attempting Connection?");
        server = client.Connect("localhost", PortNum, writer);
        // Add Reference //
        CurrentClient = this;
    }
    #region Event Methods
    #region Network Methods
    void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        failReason = ConnectionFail.IncorrectPassword;
    }

    void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        // Receive Data //
        NetworkService.InvokeNetworkReceive(this, peer, reader, channelNumber, deliveryMethod);
        //API dataType = (API)reader.GetInt();
        
        //switch(dataType)
        //{
        //    case API.Message:
        //        string message = reader.GetString();
        //        //ChatService.AddLog(message);
        //        break;
        //}
    }

    void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
    }

    void INetEventListener.OnPeerConnected(NetPeer server)
    {

    }

    void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) // Got Kicked Out By Server //
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
    #endregion
}