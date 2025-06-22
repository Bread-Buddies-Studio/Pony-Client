// Directory //
namespace Experimental;
// Imports //
using System.Diagnostics;
using Audio.Overrides;
using NAudio.Wave;

// Service //
/// <summary>
/// Handles the playback of audio.
/// </summary>
public static class AudioService
{
    // Cache //
    internal static readonly Dictionary<string, string> MediaPaths = [];
    // Constants //
    public static readonly string AudioDirectory = Path.Combine(Environment.CurrentDirectory, "Audio");
    internal static readonly List<MediaPlayer> CurrentPlayers = [];
    // Constructor //
    static AudioService()
    {
        foreach (string mediaPath in Directory.GetFiles(AudioDirectory))
        {
            // Create Uri from Path //
            Uri mediaUri = new(mediaPath);
            // Cache Media //
            /*AudioFileReader media = */CacheMedia(mediaUri);
            // Debug Log //
            Debug.WriteLine($"Cached Media: {mediaUri} as {Path.GetFileName(mediaPath)}");
        }
    }
    /// <summary>
    /// Stops all current media playing.
    /// </summary>
    public static void StopAllMedia()
    {
        // Delete All Players //
        foreach (var player in CurrentPlayers.Reverse<MediaPlayer>())
            player.Dispose();
        // Ensure List is Clear //
        CurrentPlayers.Clear();
    }

    /// <summary>
    /// Stops all media players in the provided collection <paramref name="mediaPlayers"/>.
    /// </summary>
    /// <param name="mediaPlayers">Specified collection.</param>
    public static void StopAllMedia(IEnumerable<MediaPlayer> mediaPlayers)
    {
        foreach (var player in mediaPlayers)
        {
            player.Dispose();
            CurrentPlayers.Remove(player);
        }
    }
    // Internal Methods //
    internal static WaveFileReader GetMedia(string mediaName) => new(MediaPaths[mediaName]);
    /// <summary>
    /// Caches Media to <see cref="MediaPaths"/>
    /// </summary>
    /// <param name="mediaPath">Path of the file</param>
    /// <returns></returns>
    internal static WaveFileReader CacheMedia(string mediaPath)
    {
        // Extract Name //
        string mediaName = Path.GetFileNameWithoutExtension(mediaPath);
        // Create Media //
        WaveFileReader media = new(mediaPath);
        // Cache Media //
        MediaPaths[mediaName] = mediaPath;
        // Return Media //
        return media;
    }
    /// <inheritdoc cref="CacheMedia(string)"/>
    internal static WaveFileReader CacheMedia(Uri mediaPath) => CacheMedia(mediaPath.AbsolutePath);
}