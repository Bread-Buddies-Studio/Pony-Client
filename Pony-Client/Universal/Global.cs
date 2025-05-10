using Avalonia.Threading;
using Pony_Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Universal;

public static class Global
{
    // Client //
    public static Client? client;
    // Account //
    public static byte? TagLine;
    public static string? UserName;
    public static string? PassWord;
    // Constants //
    public const int FPS = 165;
    public const int PortNum = 11_000;
    public const string HostName = "";
    // Data //
    public static readonly IPHostEntry LocalHost = string.IsNullOrEmpty(HostName) ? Dns.GetHostEntry(Dns.GetHostName()) : Dns.GetHostEntry(HostName);
    public static readonly IPAddress address = LocalHost.AddressList[0];
    public static readonly IPEndPoint serverEndPoint = new(address, PortNum);
}
