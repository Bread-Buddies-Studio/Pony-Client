using Avalonia.Controls;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Media;
using Avalonia.Media.TextFormatting.Unicode;
using Avalonia.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pony_Client.Services;

public static class AnimationService
{
    public static readonly List<Animator> Animators = [];
}

public class Animator
{
    public static readonly Dictionary<string, IImage[]> Animations = [];
    // Cancellation //
    public CancellationTokenSource cancelSource = new();
    // Frame Data //
    public IImage[]? Frames;
    public int? FrameIndex = 0;
    public IImage? Frame;
    public int FPS = 20;
    // Play Data //
    private Task? currentAnimation;
    public bool IsPlaying { get; private set; } = false;
    public bool Looped = false;
    // Constructor //
    public Animator() => AnimationService.Animators.Add(this);
    static Animator() => LoadAnimations();
    // Static Methods //
    public static IImage[] GetFrames(string animationName)
    {
        // Data //
        string directoryPath = $"avares://Pony-Client/Assets/Animations/{animationName}/";
        // Files //
        IEnumerable<Uri> fileNames = AssetLoader.GetAssets(new(directoryPath), null).OrderBy(frame => frame.AbsoluteUri);
        // Extract Images //
        IImage[] frames = fileNames.Select<Uri, IImage>(filePath => GuiService.LoadFromResource(filePath)).ToArray();
        // Debug //
        Debug.WriteLine($"Finding frames in {directoryPath}! Contains {fileNames.Count()}!");
        foreach ((int index, var frame) in frames.Index())
            Debug.WriteLine($"Found Frame x{index}!");
        // Load Image //
        return frames;
    }
    // Coroutines //
    private IEnumerable Play(IImage[] animation)
    {
        do
        {
            foreach ((int index, IImage frame) in animation.Index())
            {
                // Settings //
                Frame = frame;
                FrameIndex = index;
                // Delay //
                long delayStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                // Delay Based on FPS //
                while (true)
                {
                    // Data //
                    long delayEnd = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    // Conditions //
                    if (cancelSource.IsCancellationRequested)
                        yield break;
                    if (delayEnd - delayStart >= 1000 / FPS)
                        break;
                    // Delay //
                    yield return null;
                }
            }
        } while (Looped);
    }
    // Base Methods //
    public async Task Play(string animationName)
    {
        await Stop();
        // Checks //
        if (currentAnimation is not null)
            await currentAnimation;
        cancelSource.Dispose();
        cancelSource = new CancellationTokenSource();
        // Debug //
        //Debug.WriteLine(cancelSource.IsCancellationRequested);
        // Initialize //
        IsPlaying = true;
        // Frames //
        Frames = Animations[animationName];// GetFrames(animationName);
        // Delay until Coroutine //
        currentAnimation = Task.Run(async () => await CoroutineService.StartCoroutine(Play(Frames)));
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
        // Directory //
        Uri directoryPath = new("avares://Pony-Client/Assets/Animations/");
        // Order The Animations Alphabetically & Numerically //
        IOrderedEnumerable<Uri> enumerator = AssetLoader.GetAssets(directoryPath, null).OrderBy(frame => 
        {
            string imagePath = frame.AbsoluteUri;
            int indexBegin = imagePath.LastIndexOf('-') + 1;
            int indexEnd = imagePath.LastIndexOf('.');
            string frameIndex = imagePath[indexBegin..indexEnd];
            string fileName = imagePath.Replace(frameIndex + ".png", $"{int.Parse(frameIndex):D5}");
            return fileName;
        });
        // Store Animations //
        foreach (Uri imagePath in enumerator)
        {
            string pathString = imagePath.AbsolutePath.Replace(directoryPath.AbsolutePath, "");
            string animationName = pathString[..pathString.IndexOf('/')];

            Animations.TryGetValue(animationName, out IImage[]? frameArray);

            IImage newFrame = GuiService.LoadFromResource(imagePath);

            if (frameArray is null)
                Animations[animationName] = [newFrame];
            else
                Animations[animationName] = [.. frameArray, GuiService.LoadFromResource(imagePath)];
        }
    }
}