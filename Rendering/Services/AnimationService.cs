using Avalonia.Animation;
using Avalonia.Controls.Platform;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Reactive;
using Rendering.Views;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using Core.Services;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media.Imaging;
using Rendering.Structs;
using Avalonia;
using System.Text.Json.Serialization.Metadata;
using Core.Extensions;

namespace Rendering.Services;

public static class AnimationService
{
    public static readonly List<Animator> Animators = [];
}

public class Animator
{
    private static readonly Uri directoryPath = new("avares://Rendering/Assets/Animations/");
    public static readonly Dictionary<string, SheetData> Sheets = [];
    // Play Data //
    [MemberNotNullWhen(true, nameof(Frame), nameof(CurrentAnimation), nameof(FrameIndex))]
    public bool IsPlaying => CurrentAnimation?.IsPlaying ?? false;
    private Animation? CurrentAnimation;
    // Frame Data //
    [DisallowNull]
    public int? FrameIndex
    {
        get => CurrentAnimation?.FrameIndex;
        set => CurrentAnimation?.SetFrame((int)value);
    }
    public IImage? Frame => CurrentAnimation?.CurrentFrame;
    public byte? Layer => CurrentAnimation?.Layer;
    // Constructor //
    public Animator() => AnimationService.Animators.Add(this);
    static Animator() => LoadAnimations();
    // Base Methods //
    public void Play(string sheetName, string animationName, int degrees = 0)
    {
        // Get Animation //
        if (!Sheets.TryGetValue(sheetName, out SheetData sheetData))
        {
            Debug.WriteLine($"Animation Sheet '{sheetName}' not found");
            return;
        }
        // Save Animation
        Animation? currentAnimation;
        // Get Split Amount //
        int directionalAmount = 8; // Animations are 8 directional
        // Round Degrees //
        degrees = degrees.RoundToDegrees(directionalAmount);
        // Get Direction //
        if (!sheetData.Directions.TryGetValue(degrees, out Animation[]? animations))
        {
            Debug.WriteLine($"Animation '{animationName}' not found in sheet '{sheetName}' at {degrees} degrees");
            return;
        }
        // Find Animation //
        if ((currentAnimation = animations.FirstOrDefault(currentAnimation => currentAnimation.Name == animationName)) is null)
        {
            Debug.WriteLine($"Animation '{animationName}' not found in sheet '{sheetName}' at {degrees} degrees");
            return;
        }
        // Conditions //
        if (currentAnimation == CurrentAnimation)
            return; // Don't waste time playing something already playing //
        // Debug // 
        //Debug.WriteLine("Playing: " + animationName + " | " + degrees + " degrees");
        // Stop Current Animation //
        Stop();
        // Play Animation //
        CurrentAnimation = currentAnimation;
        CurrentAnimation.Play();
    }
    public void Stop() => CurrentAnimation?.Stop();
    // Private Base Methods //
    private static void LoadAnimations()
    {
        IEnumerable<Uri> animationData = AssetLoader.GetAssets(directoryPath, null)
            .Where(data =>
            {
                string dataPath = data.AbsoluteUri;
                // Conditions //
                return dataPath.EndsWith(".json");
            });

        // Order The Animation Frames Alphabetically & Numerically //
        IEnumerable<Uri> frames = AssetLoader.GetAssets(directoryPath, null)
            .Where(frame =>
            {
                string dataPath = frame.AbsoluteUri;
                // Conditions //
                return !dataPath.EndsWith(".json");
            });
        // Cache Animations //
        foreach (Uri dataPath in animationData)
            CacheAnimationData(dataPath);
        // Store Frames //
        foreach (Uri imagePath in frames)
            CacheAnimationFrame(imagePath);
    }

    private static void CacheAnimationData(Uri dataPath)
    {
        string pathString = dataPath.AbsolutePath.Replace(directoryPath.AbsolutePath, "");
        string sheetName = pathString[..pathString.IndexOf('/')];
        // Load Data //
        using Stream stream = AssetLoader.Open(dataPath);
        using StreamReader reader = new(stream);
        var json = reader.ReadToEnd();
        // Deserialize //
        SheetData spriteSheet = JsonSerializer.Deserialize<SheetData>(json);
        // Add Reference //
        Sheets[sheetName] = spriteSheet;
    }

    private static void CacheAnimationFrame(Uri framePath)
    {
        string pathString = framePath.AbsolutePath.Replace(directoryPath.AbsolutePath, "");
        string sheetName = pathString[..pathString.IndexOf('/')];
        // Is Sheet Missing? //
        if (!Sheets.TryGetValue(sheetName, out SheetData spriteSheet))
        {
            Debug.WriteLine($"Animation Sheet '{sheetName}' not found");
            return;
        }
        // Settings //
        spriteSheet.Path = framePath;
        // Load Frames //
        spriteSheet.Load();
    }
}

public class Animation
{
    [AllowNull]
    public string Name { get; set; }
    public bool Pixelart { get; set; } = true;
    public byte Layer { get; set; }
    public int StartIndex { get; set; }
    public int Length { get; set; }
    public Range FrameRange => StartIndex..(StartIndex+Length);
    // File Data //
    public WriteableBitmap[] frames = [];
    public bool Looped { get; set; }
    public int FPS { get; set; }
    // Properties //
    public bool IsPlaying => Stopwatch.IsRunning;
    public int FrameCount => frames.Length;
    private long MillisSincePlay => Stopwatch.ElapsedMilliseconds + SkippedMillis;
    // Instance Data //
    private readonly Stopwatch Stopwatch = new();
    private long SkippedMillis = 0;
    public virtual int FrameIndex => (int)(MillisSincePlay / 1000.0 * FPS) % FrameCount;
    public IImage CurrentFrame => frames[FrameIndex];
    // Base Methods //
    public void SetFrame(int frameIndex)
    {
        // Reset to first Frame //
        Stop();
        // Skip to desired frame //
        AdvanceFrame(frameIndex);
    }
    public void AdvanceFrame(int advanceAmount = 1) => SkippedMillis += (long)(1000 * advanceAmount / 1000.0) * FPS;
    public void Play()
    {
        Stopwatch.Restart();
        SkippedMillis = 0;
    }
    public void Pause() => Stopwatch.Stop();
    public void Stop()
    {
        Stopwatch.Reset();
        SkippedMillis = 0;
    }
    public void Resume() => Stopwatch.Start();
    // Loading Methods //
    public void Load(SheetData sheetData, Bitmap Sheet)
    {
        Debug.WriteLine("Loading Animation: " + Name);
        // Allocate Memory //
        frames = new WriteableBitmap[Length];
        // Store //
        for (int frameIndex = StartIndex, iterationIndex = 0; frameIndex < StartIndex + Length; frameIndex++, iterationIndex++)
        {
            int column = frameIndex % sheetData.ColumnAmount;
            int row = frameIndex / sheetData.ColumnAmount;
            
            PixelSize frameSize = new(sheetData.SizeWithoutPadding.Width / sheetData.ColumnAmount, 
                sheetData.SizeWithoutPadding.Height / sheetData.RowAmount);

            PixelPoint topLeft = new(
                sheetData.EdgePixelPadding + // Edge Padding //
                sheetData.FramePixelPadding * column + // Frame Seperator Padding //
                column * frameSize.Width, // Column Offset //

                sheetData.EdgePixelPadding + // Edge Padding //
                sheetData.FramePixelPadding * row + // Frame Seperator Padding //
                row * frameSize.Height // Row Offset //
                );
            PixelRect pixelRect = new(topLeft, frameSize);
            // Extract Frame //
            WriteableBitmap frame = GuiService.CropBitmap(Sheet, pixelRect);
            // Store it //
            frames[iterationIndex] = frame;
        }
        // Fix Blurry Images //
    }

    public void UnLoad()
    {
        // Dispose Frames //
        foreach (WriteableBitmap frame in frames)
            frame.Dispose();
        // Clear Frames //
        frames = [];
        // Reset State //
        Stop();
    }
}