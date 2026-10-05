using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Core.Services;
using Rendering.Views;
using TiledSharp;
using Core.Extensions;
using Rendering.Services;
using System.Diagnostics;
using Rendering.Entities;
using Avalonia.Controls;
using System.Reflection.Metadata.Ecma335;
using Core.Interfaces;

namespace Rendering.Visible;

public class Tilemap : IPhysical
{
    // Arrays //
    private readonly Structure[] wallArray = [];
    // Override //
    public Rect Hitbox
    {
        get => new(Position, Size);
        set => Position = value.Position;
    }
    public Point Position { get; set; }
    public Size Size => new(ResourceFile.Width, ResourceFile.Height);
    // Render Options //
    public Matrix RenderMatrix
    {
        get
        {
            // Condtitions //
            if (WorldService.CurrentCamera is null)
                return Matrix.Identity;
            // Data //
            Matrix matrix = WorldService.CurrentCamera.OffsetMatrix
                .Append(Matrix.CreateTranslation(Position));
            // Return //
            return matrix;
        }
    }
    // Public Data //
    public required Uri ResourcePath;
    public required TmxMap ResourceFile;
    public required Bitmap Tileset;

    public required int TileWidth { get; init; }
    public required int TileHeight { get; init; }
    public required int ColumnAmount { get; init; }
    public required int RowAmount { get; init; }
    // Private Data //
    private readonly WriteableBitmap[] tileCache;
    // Constructors //
    [SetsRequiredMembers]
    public Tilemap(string mapName, Point Position = new())
    {
        // Init //
        this.ResourcePath = new Uri(TilemapService.Directory + mapName + ".tmx");
        this.ResourceFile = StorageService.LoadTilemapResource(TilemapService.Directory + mapName + ".tmx");
        this.Tileset = StorageService.LoadBitmapResource(TilemapService.Directory + ResourceFile.Tilesets[0].Name + ".png");
        //
        this.TileWidth = ResourceFile.TileWidth;
        this.TileHeight = ResourceFile.TileHeight;

        this.Position = Position;

        ColumnAmount = Tileset.PixelSize.Width / TileWidth;
        RowAmount = Tileset.PixelSize.Height / TileHeight;

        tileCache = new WriteableBitmap[ColumnAmount * RowAmount];

        // Cache Tile Images //
        for (int i = 0; i < tileCache.Length; i++)
        {
            int x = (i % ColumnAmount) * TileWidth;
            int y = (i / ColumnAmount) * TileHeight;
            tileCache[i] = GuiService.CropBitmap(Tileset, 
                new PixelRect(
                    new PixelPoint(x, y), 
                    new PixelSize(TileWidth, TileHeight)
                )
            );
        }
        // Go Through Layers //
        foreach (TmxLayer layer in ResourceFile.Layers.Reverse())
        {
            // Check for Collision //
            bool isWallLayer = layer.Name.ToLower() is "walls";

            foreach ((int index, TmxLayerTile tile) in layer.Tiles
                .Index())
            {
                // Conditions //
                if (tile.Gid is 0)
                    continue;
                // Data //
                int frameIndex = tile.Gid - 1; // Tiled uses 1-based indexing for tiles
                float x = (index % ResourceFile.Width) * ResourceFile.TileWidth;
                float y = (float)Math.Floor(index / (double)ResourceFile.Width) * ResourceFile.TileHeight;
                // Collisions //
                //Debug.WriteLine("X: " + this.Position.X);
                //Debug.WriteLine("Y: " + this.Position.Y);
                _ = new Structure(
                    tileCache[frameIndex],

                    new Rect(
                        new Point(x + this.Position.X, y + this.Position.Y),
                        new Size(TileWidth, TileHeight)
                    )
                )

                {
                    IsCollidable = isWallLayer,
                    IsAnchored = true,
                };
            }
        }
    }
}
