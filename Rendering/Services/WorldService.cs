using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Audio.Services;
using Avalonia.Media;
using Core.Extensions;
using Core.Geometry;
using Core.Services;
using Rendering.Effects;
using Rendering.Entities;
using Rendering.Views;
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
        foreach ((string directoryName, Uri[] assets) in StorageService.GetAssets(new("avares://Rendering/Assets/")))
        {
            Debug.WriteLine($"{directoryName}: ");
            foreach (Uri asset in assets)
                Debug.WriteLine($"\t{asset.OriginalString}");
            Debug.WriteLine("");
        }

        var map = new Tilemap("TestMap", Position: new(-500, -500));
        
        _ = new NPC("Pinkamena", new(400, 0), new(200, 200))
        {
            Layer = 1,
            PBL = true
        };

        //_ = new Structure(new(100, 100, 100, 100))
        //{ 
        //    IsCollidable = true 
        //};

        //int treeAmount = 1;

        //for (int i = 1; i <= treeAmount; i++)
        //{
        //    _ = new Structure(new(1100 * i, 0, 1000, 1000), "tree")
        //    {
        //        Layer = (byte)i,
        //        PBL = true
        //    };
        //}

        //Debug.WriteLine(DialogueService.Writer);
    }
}