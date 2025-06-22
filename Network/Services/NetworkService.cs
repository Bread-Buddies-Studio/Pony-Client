using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;

namespace Network.Services;

public static class NetworkService
{
    // Events //
    public static event EventHandler<(NetPeer, NetPacketReader, byte, DeliveryMethod)>? NetworkReceive;
    // Invoke Methods //
    internal static void InvokeNetworkReceive(Client client, NetPeer server, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod) =>
        NetworkReceive?.Invoke(client, (server, reader, channelNumber, deliveryMethod));
}
