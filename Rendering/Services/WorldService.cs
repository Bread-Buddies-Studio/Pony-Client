using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rendering.Entities;

namespace Rendering.Services;

public static class WorldService
{
    // Data //
    public static LocalPlayer? LocalPlayer { get; set; }
    // Constructor //
    static WorldService()
    {
        for (int i = 1; i <= 10_000; i++)
        {
            _ = new Structure(new(1100 * i, 0, 1000, 1000), "tree")
            {
                Layer = (byte)i,
                PBL = true
            };
        }
    }
}