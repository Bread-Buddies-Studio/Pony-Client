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
using Core.Structs;
using Rendering.Effects;
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

        //_ = new Structure(new(500, 500, 200, 200))
        //{
        //    IsAnchored = true,
        //    IsCollidable = true
        //};

        //_ = new Structure(new(0, 500, 200, 200))
        //{
        //    IsAnchored = false,
        //    IsCollidable = true
        //};


        //Typewriter typewriter = new(
        //    new SolidColorBrush(Color.FromRgb(255, 255, 255)),
        //    Typeface.Default,
        //    new(500, 500, 500, 1000))
        //{
        //    Text = "Hello World!"
        //};

        //Task.Run(async () =>
        //{
        //    while (true)
        //    {
        //        typewriter.Text += " 1";
        //        await Task.Delay(50);
        //    }
        //});

        //Debug.WriteLine(@"[b]Bold Text![\b]".RemoveBBCode());

        //foreach (Match match in @"[b]Bold Text![\b]".ParseBBCode())
        //{
        //    Debug.WriteLine($"Index: {match.Index} | Value: {match.Value}");
        //}

        Debug.WriteLine(DialogueService.Writer);
    }
}