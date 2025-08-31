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

namespace Rendering.Visible;

public class Tilemap : Renderable
{
    // Arrays //
    private readonly Structure[] wallArray = [];
    // Override //
    public override Rect Hitbox
    {
        get => new(Position, Size);
        set => Position = value.Position;
    }
    public override Point Position { get; set; } = new();
    public override Size Size => new(ResourceFile.Width, ResourceFile.Height);
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
    public override bool AlwaysShows => true;
    public override bool IsAlwaysOnTop => false;
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
    // Constructor //
    [SetsRequiredMembers]
    public Tilemap(string mapName)
    {
        // Init //
        this.ResourcePath = new Uri(TilemapService.Directory + mapName + ".tmx");
        this.ResourceFile = StorageService.LoadTilemapResource(TilemapService.Directory + mapName + ".tmx");
        this.Tileset = StorageService.LoadBitmapResource(TilemapService.Directory + ResourceFile.Tilesets[0].Name + ".png");
        //
        TileWidth = ResourceFile.TileWidth;
        TileHeight = ResourceFile.TileHeight;

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
        foreach (TmxLayer layer in ResourceFile.Layers)
        {
            // Check //
            bool isWallLayer = layer.Name.ToLower() is "walls";

            foreach ((int index, TmxLayerTile tile) in layer.Tiles
                .Where(static tile => tile.Gid is not 0)
                .Index())
            {
                Debug.WriteLine("Graphical ID: " + tile.Gid + '\n');
                
                int frameIndex = tile.Gid - 1; // Tiled uses 1-based indexing for tiles

                float x = (index % ResourceFile.Width) * ResourceFile.TileWidth;
                float y = (float)Math.Floor(index / (double)ResourceFile.Width) * ResourceFile.TileHeight;

                //_ = new Structure(
                //    tileCache[frameIndex],

                //    new Rect(
                //        new Point(x, y),
                //        new Size(TileWidth, TileHeight)
                //    )
                //)

                //{
                //    IsCollidable = isWallLayer,
                //    IsAnchored = true,
                //};
            }
        }
    }
    // Event Methods //
    public override void OnRender(GuiService guiService, DrawingContext context)
    {
        // Offset //
        PushTransform(context, RenderMatrix);
        // Draw Each Layer //
        foreach (TmxLayer layer in ResourceFile.Layers)
        {
            foreach ((int index, TmxLayerTile tile) in layer.Tiles
                .Where(static tile => tile.Gid is not 0)
                .Index())
            {
                int frameIndex = tile.Gid - 1; // Tiled uses 1-based indexing for tiles

                float x = (index % ResourceFile.Width) * ResourceFile.TileWidth;
                float y = (float)Math.Floor(index / (double)ResourceFile.Width) * ResourceFile.TileHeight;

                context.DrawImage(
                    tileCache[frameIndex],
                    new Rect(
                        new Point(x, y),
                        new Size(TileWidth, TileHeight)
                    )
                );
            }
        }
        // Reset Offset Matrix //
        PopMatrix();
    }
}
