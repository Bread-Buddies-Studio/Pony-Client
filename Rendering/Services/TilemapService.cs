using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Services;
using Rendering.Entities;
using Rendering.Visible;
using TiledSharp;

namespace Rendering.Services;

public static class TilemapService
{
    internal const string Directory = "avares://Rendering/Assets/Tilemaps/";

    public static TmxMap LoadTilemap(string MapName)
    {
        return StorageService.LoadTilemapResource(Directory + MapName);
    }
}
