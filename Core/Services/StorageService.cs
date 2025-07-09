using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using NAudio.Wave;

namespace Core.Services;

public static class StorageService
{
    static StorageService()
    {
        
    }

    public static Bitmap LoadBitmapResource(Uri resourcePath)
    {
        try
        {
            return new(AssetLoader.Open(resourcePath));
        }

        catch (FileNotFoundException exception)
        {
            Console.WriteLine($"Resource not found: {resourcePath}. Exception: {exception.Message}");
            return new Bitmap(AssetLoader.Open(new Uri("avares://Core/Assets/Images/NotFound.png")));
        }
    }
    public static Bitmap LoadBitmapResource(string resourcePath) => LoadBitmapResource(new Uri(resourcePath));

    public static WaveFileReader LoadAudioResource(Uri resourcePath)
    {
        try
        {
            return new(AssetLoader.Open(resourcePath));
        }

        catch (FileNotFoundException exception)
        {
            Console.WriteLine($"Resource not found: {resourcePath}. Exception: {exception.Message}");
            return new WaveFileReader(AssetLoader.Open(new Uri("avares://Core/Assets/Images/NotFound.png")));
        }
    }
    public static WaveFileReader LoadAudioResource(string resourcePath) => LoadAudioResource(new Uri(resourcePath));
}
