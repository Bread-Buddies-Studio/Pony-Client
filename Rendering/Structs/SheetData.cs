using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Core.Services;
using Rendering.Services;

namespace Rendering.Structs;

public struct SheetData()
{
    public Dictionary<int, Animation[]> Directions { get; init; } = [];

    public Animation[] Animations
    {
        get
        {
            // Conditions //
            if (field.Length > 0)
                return field;
            // Initialize Field //
            field = [];
            // Add Animations //
            foreach (Animation[] animationArray in Directions.Values)
                field = [.. field, .. animationArray];
            // Return Field //
            return field;
        }
    } = [];
    public Bitmap? Sheet { get; set; }
    public readonly Size SizeWithoutPadding
    {
        get
        {
            // Conditions //
            if (Sheet is null)
                return Size.Empty;
            // Return Size //
            return new Size(Sheet.PixelSize.Width - EdgePixelPadding * 2 - FramePixelPadding * (ColumnAmount - 1), 
                Sheet.PixelSize.Height - EdgePixelPadding * 2 - FramePixelPadding * (RowAmount - 1));
        }
    }
    public int ColumnAmount { get; set; }
    public int RowAmount { get; set; }
    [AllowNull]
    public Uri Path { get; set; }
    public readonly string PathString => Path.AbsolutePath;
    public bool IsLoaded { get; private set; } = false;

    public int EdgePixelPadding { get; set; }
    public int FramePixelPadding { get; set; }

    // Base Methods //
    public void Load()
    {
        // Conditions //
        if (IsLoaded)
            return;
        // Load Sheet //
        Sheet = StorageService.LoadBitmapResource(Path);
        // Load Data //
        Debug.WriteLine("Column Amount: " + this.ColumnAmount);
        Debug.WriteLine("Row Amount: " + this.RowAmount);
        foreach (Animation animation in Animations)
            animation.Load(this, Sheet);

        Sheet.Dispose();
        // Save State //
        IsLoaded = true;
    }
}