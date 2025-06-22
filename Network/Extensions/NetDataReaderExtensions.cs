using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib.Utils;

namespace Network.Extensions;

public static class NetDataReaderExtensions
{
    public static void SkipByte(this NetDataReader reader) => reader.SkipBytes(1);
    public static int Advance(this NetDataReader reader)
    {
        reader.SetPosition(reader.Position + 1);
        return reader.Position;
    }
}
