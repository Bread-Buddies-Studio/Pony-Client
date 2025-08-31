using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using static SkiaSharp.SKImageFilter;

namespace Core.Extensions;

public static class BitmapExtensions
{
    public static WriteableBitmap CopyPixels(this Bitmap source, PixelRect cropRect)
    {
        WriteableBitmap croppedBitmap = new(cropRect.Size, source.Dpi);

        using ILockedFramebuffer frameBuffer = croppedBitmap.Lock();
        
        source.CopyPixels(
            sourceRect: cropRect,
            buffer: frameBuffer.Address,
            bufferSize: frameBuffer.RowBytes * frameBuffer.Size.Height,
            stride: frameBuffer.RowBytes
        );

        return croppedBitmap;
    }
}