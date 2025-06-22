using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib.Utils;

namespace Network.Extensions;

public static class INetSerializableExtensions
{
    /// <summary>
    /// Warning: Inefficient, better to transfer data from the network directly to <paramref name="otherSerializable"/>
    /// </summary>
    /// <param name="currentSerializable"></param>
    /// <param name="otherSerializable"></param>
    public static void TransferData(this INetSerializable currentSerializable, INetSerializable otherSerializable)
    {
        // Initialize Writer //
        NetDataWriter writer = new();
        // Add Data to Writer //
        currentSerializable.Serialize(writer);
        // Create Reader from Writer //
        NetDataReader reader = new(writer);
        otherSerializable.Deserialize(reader);
    }
}
