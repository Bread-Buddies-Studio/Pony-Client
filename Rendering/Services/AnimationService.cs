using Avalonia.Controls;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting.Unicode;
using Avalonia.Platform;
using Avalonia.Reactive;
using Rendering.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rendering.Services;

public static class AnimationService
{
    public static readonly List<Animator> Animators = [];
}

public class Animator
{
    private static readonly Uri directoryPath = new("avares://Rendering/Assets/Animations/");
    public static readonly Dictionary<string, Animation> Animations = [];
    // Cancellation //
    public CancellationTokenSource cancelSource = new();
    // Frame Data //
    public IImage[]? Frames;
    public int? FrameIndex = 0;
    public IImage? Frame;
    public int FPS = 24;
    // Play Data //
    private Task? currentAnimation;
    public bool IsPlaying { get; private set; } = false;
    public bool Looped = false;
    // Constructor //
    public Animator() => AnimationService.Animators.Add(this);
    static Animator() => LoadAnimations();
    // Coroutines //
    private async Task Play(IImage[] animation)
    {
        // Play Frames //
        do
        {
            foreach ((int index, IImage frame) in animation.Index())
            {
                // Pre-Conditions //
                if (cancelSource.IsCancellationRequested)
                    return;
                // Settings //
                Frame = frame;
                FrameIndex = index;
                // Post-Conditions //
                if (animation.Length is 1)
                    return;
                // Delay Based on FPS //
                try
                {
                    await Task.Delay(1000 / FPS, cancelSource.Token);
                }
                // Animation Stopped //
                catch (TaskCanceledException) { return; }
            }
        } while (Looped);
    }
    // Base Methods //
    public async Task Play(string animationName)
    {
        await Stop();
        // Checks //
        cancelSource.Dispose();
        cancelSource = new CancellationTokenSource();
        // Initialize //
        IsPlaying = true;
        // Frames //
        Frames = Animations[animationName].frames; // GetFrames(animationName);
        // Conditions //
        if (Frames is null)
            return;
        // Delay until Coroutine //
        currentAnimation = Task.Run(() => Play(Frames));
        await currentAnimation;
    }
    public async Task Stop()
    {
        IsPlaying = false;
        FrameIndex = 0;
        // Cancel //
        await cancelSource.CancelAsync();
    }
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
        // Check if Deserialize Succeeded //
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
        if (animation.frames is null) 
            return;
        // Load Frame //
        IImage frame = GuiService.LoadFromResource(framePath);
        // Save //
        animation.frames = [.. animation.frames, frame];
    }
}


public class Animation
{
    public IImage[] frames = [];
    public required bool looped;
    public required int fps;
}