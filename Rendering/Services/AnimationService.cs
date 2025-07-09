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

namespace Rendering.Services;

public static class AnimationService
{
    public static readonly List<Animator> Animators = [];
}

public class Animator
{
    private static readonly Uri directoryPath = new("avares://Rendering/Assets/Animations/");
    public static readonly Dictionary<string, Animation> Animations = [];
    // Play Data //
    private Animation? CurrentAnimation;
    public bool IsPlaying => CurrentAnimation?.IsPlaying ?? false;
    public bool Looped = false;
    // Frame Data //
    public int? FrameIndex { get => CurrentAnimation?.FrameIndex; }
    public IImage? Frame { get => CurrentAnimation?.CurrentFrame; }
    // Constructor //
    public Animator() => AnimationService.Animators.Add(this);
    static Animator() => LoadAnimations();
    // Base Methods //
    public void Play(string animationName)
    {
        Stop();
        // Get Animation //
        if (!Animations.TryGetValue(animationName, out Animation? currentAnimation))
            return;
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
        IOrderedEnumerable<Uri> frames = AssetLoader.GetAssets(directoryPath, null)
            .Where(frame =>
            {
                string dataPath = frame.AbsoluteUri;
                // Conditions //
                return !dataPath.EndsWith(".json");
            })

            .OrderBy(frame =>
            {
                string imagePath = frame.AbsoluteUri;
                int indexBegin = imagePath.LastIndexOf('-') + 1;
                int indexEnd = imagePath.LastIndexOf('.');
                string frameIndex = imagePath[indexBegin..indexEnd];
                string fileName = imagePath.Replace(frameIndex + ".png", $"{int.Parse(frameIndex):D5}");
                return fileName;
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
        string animationName = pathString[..pathString.IndexOf('/')];
        // Load Data //
        using Stream stream = AssetLoader.Open(dataPath);
        using StreamReader reader = new(stream);
        var json = reader.ReadToEnd();
        // Deserialize //
        Animation? animation = JsonSerializer.Deserialize<Animation>(json);
        // Conditions //
        if (animation is null)
            return;
        // Save //
        Animations[animationName] = animation;
    }

    private static void CacheAnimationFrame(Uri framePath)
    {
        string pathString = framePath.AbsolutePath.Replace(directoryPath.AbsolutePath, "");
        string animationName = pathString[..pathString.IndexOf('/')];
        // Does Animation Already Exist? //
        if (!Animations.TryGetValue(animationName, out Animation? animation))
            return;
        // Load Frame //
        IImage frame = StorageService.LoadBitmapResource(framePath);
        // Save //
        animation.frames = [.. animation.frames, frame];
    }
}

public class Animation
{
    // File Data //
    public IImage[] frames = [];
    public bool Looped { get; set; }
    public int FPS { get; set; }
    // Properties //
    public bool IsPlaying => Stopwatch.IsRunning;
    public int FrameCount => frames.Length;
    private long MillisSincePlay => Stopwatch.ElapsedMilliseconds + SkippedMillis;
    // Instance Data //
    private readonly Stopwatch Stopwatch = new();
    private long SkippedMillis = 0;
    public int FrameIndex => (int)(MillisSincePlay / 1000.0 * FPS) % FrameCount;
    public IImage CurrentFrame => frames[FrameIndex];
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
}