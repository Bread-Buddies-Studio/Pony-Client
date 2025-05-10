using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Core.Extensions;
using LiteNetLib.Utils;
using Network.Enums;

namespace Network.Structs;

public struct NetworkData(API Reason) : INetSerializable
{
    // API //
    public required API Reason = Reason;
    // Private Data //
    private long _deserializationTime;
    // Public Data //
    public required string Message = string.Empty;
    public required Point Position = new();
    #region Network Methods
    public readonly void Serialize(NetDataWriter writer)
    {
        writer.Put((int)Reason);
        writer.Put(Message);
        writer.PutArray([Position.X, Position.Y]);
    }
    public void Deserialize(NetDataReader reader)
    {
        // Private Data //
        this._deserializationTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        // Deserialize //
        this.Reason = (API)reader.GetInt();
        this.Message = reader.GetString();
        this.Position = PointExtensions.FromDoubleArray(reader.GetDoubleArray());
    }
    #endregion
}