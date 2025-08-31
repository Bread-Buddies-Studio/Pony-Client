using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using NAudio.Wave;
using TiledSharp;

namespace Core.Services;

public static class StorageService
{
    public static Bitmap LoadBitmapResource(Uri resourcePath)
    {
        try
        {
            return new(AssetLoader.Open(resourcePath));
        }

        catch (FileNotFoundException exception)
        {
            Debug.WriteLine($"Resource not found: {resourcePath}. Exception: {exception.Message}");
            return new Bitmap(AssetLoader.Open(new Uri("avares://Rendering/Assets/Images/NotFound.png")));
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

    public static TmxMap LoadTilemapResource(Uri resourcePath)
    {
        try
        {
            return new TmxMap(AssetLoader.Open(resourcePath));
        }

        catch (FileNotFoundException exception)
        {
            Debug.WriteLine($"Tilemap not found: {resourcePath}. Exception: {exception.Message}");
            return new TmxMap(AssetLoader.Open(new Uri("avares://Rendering/Assets/Tilemaps/NotFound.tmx")));
        }
    }
    public static TmxMap LoadTilemapResource(string resourcePath) => LoadTilemapResource(new Uri(resourcePath));
}
