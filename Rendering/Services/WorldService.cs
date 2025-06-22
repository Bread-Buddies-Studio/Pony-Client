using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Audio.Services;
using Core.Geometry;
using Core.Services;
using Core.Structs;
using Rendering.Entities;
using Rendering.Visible;

namespace Rendering.Services;

public static class WorldService
{
    // Data //
    public static LocalPlayer? LocalPlayer { get; set; }
    public static Camera? CurrentCamera => LocalPlayer?.AttachedCamera;
    // Constructor //
    static WorldService()
    {
        Line.Main();

        int treeAmount = 1;

        for (int i = 1; i <= treeAmount; i++)
        {
            _ = new Structure(new(1100 * i, 0, 1000, 1000), "tree")
            {
                Layer = (byte)i,
                PBL = true
            };
        }

        _ = new Structure(new(500, 500, 200, 200))
        {
            IsAnchored = true,
            IsCollidable = true
        };

        _ = new Structure(new(0, 500, 200, 200))
        {
            IsAnchored = false,
            IsCollidable = true
        };

    }
}