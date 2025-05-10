// Global Imports //
global using static Network.Universal.Global;
// Local Imports //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Universal;

public static class Global
{
    public static Client? CurrentClient { get; set; }
    public static int PortNum { get; set; } = 11_000;
}