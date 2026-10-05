using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using Avalonia.Controls;
using SkiaSharp;

namespace Core.Extensions;

public static class WritableBitmapExtensions
{
    public static Color GetPixel(this WriteableBitmap source, PixelPoint point)
    {
        // Conditions //
        if (source is null)
            throw new ArgumentNullException(nameof(source), "Source bitmap cannot be null.");
        
        if (point.X < 0 || point.Y < 0 || point.X >= source.PixelSize.Width || point.Y >= source.PixelSize.Height)
            throw new ArgumentOutOfRangeException(nameof(point), "Point is outside the bounds of the bitmap.");
        // Data //
        int sourceWidth = source.PixelSize.Width;
        int sourceHeight = source.PixelSize.Height;
        // Lock the framebuffer to access pixel data //
        using ILockedFramebuffer frameBuffer = source.Lock();
        
        int pixelIndex = point.Y * frameBuffer.RowBytes + point.X * 4;
        
        unsafe
        {
            // Convert //
            void* address = (void*)frameBuffer.Address;
            // Create a Span<byte> to access the pixel data //
            Span<byte> buffer = new(address, sourceWidth * sourceHeight * 4); // 4 bytes per pixel (BGRA)
            // Extract BGRA values //
            byte blue = buffer[pixelIndex];
            byte green = buffer[pixelIndex + 1];
            byte red = buffer[pixelIndex + 2];
            byte alpha = buffer[pixelIndex + 3];
            // Create Color //
            return Color.FromArgb(alpha, red, green, blue);
        }
    }

    public static Color GetPixel(this WriteableBitmap source, int x, int y) => source.GetPixel(new(x, y));

    public static Color[] GetColors(this WriteableBitmap source)
    {
        // Shorten Variable Names //
        int width = source.PixelSize.Width;
        int height = source.PixelSize.Height;
        // Create Array //
        HashSet<Color> colors = [];
        // Check all pixels //
        for (int x = 0, y = 0;; x++)
        {
            // Checks //
            if (x >= width)
            { 
                x = 0; 
                y++; 
            }
            // Pre-Conditions //
            if (y >= height)
                break;
            Color color = source.GetPixel(x, y);
            // Conditions //
            if (color.A is 0)
                continue; // Skip transparent pixels
            // Extract //
            colors.Add(source.GetPixel(x, y));
        }
        // Convert to Array //
        return [.. colors];
    }

    public static WriteableBitmap Overlay(this WriteableBitmap source, 
        SKEncodedImageFormat imageFormat = SKEncodedImageFormat.Png, 
        params Span<WriteableBitmap> overlays)
    {
        // Convert Source to SKBitmap //
        using SKBitmap sourceSK = (SKBitmap)source.Lock();
        // Add Overlays to Source //
        foreach (WriteableBitmap overlay in overlays)
        {
            // Convert to Skia //
            using SKBitmap overlaySK = (SKBitmap)overlay.Lock();
            // Overlay onto source //
            overlaySK.CopyTo(sourceSK);
        }
        // Encode Data //
        SKData encodeData = sourceSK.Encode(imageFormat, quality: 100);
        // Connect to Data //
        using Stream stream = encodeData.AsStream();
        // Convert Stream to Writeable Bitmap //
        return WriteableBitmap.Decode(stream);
    }
}
