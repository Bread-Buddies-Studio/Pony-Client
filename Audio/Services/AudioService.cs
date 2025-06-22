// Directory //
namespace Audio.Services;
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
    internal static readonly List<WaveOutEvent> CurrentPlayers = [];
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
    // Public Methods //
    /// <summary>
    /// Creates a new <see cref="WaveOutEvent"/> sound player for the specified media file. 
    /// </summary>
    /// <param name="mediaName"></param>
    /// <param name="playbackTime"></param>
    /// <param name="loopBegin"></param>
    /// <param name="loopEnd"></param>
    /// <param name="looped"></param>
    /// <returns></returns>
    public static WaveOutEvent CreateSoundPlayer(string mediaName, TimeSpan playbackTime = new(), long loopBegin = 0, long? loopEnd = null, bool looped = false)
    {
        // Create new Media Player //
        WaveOutEvent mediaPlayer = new();
        // Add Reference //
        CurrentPlayers.Add(mediaPlayer);
        // Find Media //
        WaveFileReader requestedMedia = GetMedia(mediaName);
        // Set Playback Time //
        requestedMedia.CurrentTime = playbackTime; // Set Playback Time if specified
        using LoopStream loopStream = new(requestedMedia, LoopBegin: loopBegin, LoopEnd: loopEnd);
        // Play Media //
        mediaPlayer.Init(looped ? loopStream : requestedMedia);
        // Finish //
        return mediaPlayer;
    }
    /// <summary>
    /// Plays a Media file by its name from the <see cref="MediaPaths"/>.
    /// </summary>
    /// <param name="mediaName">Name of the cached file path in <see cref="MediaPaths"/></param>
    /// <param name="playbackTime">Initial playback time of the audio.</param>
    /// <returns>
    /// <see cref="MediaPlayer"/> used to handle playback of the audio.
    /// </returns>
    public static WaveOutEvent Play(string mediaName, TimeSpan playbackTime = new(), long loopBegin = 0, long? loopEnd = null, bool looped = false, float Volume = .5f)
    {
        // Create Media Player //
        WaveOutEvent mediaPlayer = CreateSoundPlayer(mediaName, playbackTime, loopBegin, loopEnd, looped);
        mediaPlayer.Volume = Volume; // Set Default Volume
        // Play //
        mediaPlayer.Play();
        // Return Media Player //
        return mediaPlayer;
    }
    /// <inheritdoc cref="Play(string, TimeSpan, long, long?, bool)">
    public static WaveOutEvent Play(string mediaName, long playbackTime = 0, long loopBegin = 0, long? loopEnd = null, bool looped = false) => 
        Play(mediaName, TimeSpan.FromMilliseconds(playbackTime), loopBegin, loopEnd, looped);
    /// <summary>
    /// Stops a specific <see cref="WaveOutEvent"/> from audio playback.
    /// </summary>
    /// <param name="mediaPlayer"></param>
    /// <returns>
    /// Whether <paramref name="mediaPlayer"/> was successfully removed from <see cref="CurrentPlayers"/>
    /// </returns>
    public static bool Stop(WaveOutEvent mediaPlayer)
    {
        // Delete //
        mediaPlayer.Dispose();
        // Remove Reference //
        return CurrentPlayers.Remove(mediaPlayer);
    }
    /// <summary>
    /// Stops all current media playing.
    /// </summary>
    /// <remarks>
    /// Warning: Experimental! Untested! Unknown if audio stops playing immediately or not.
    /// </remarks>
    public static void StopAllMedia()
    {
        // Delete All Players //
        foreach (var player in CurrentPlayers.Reverse<WaveOutEvent>())
            player.Dispose();
        // Ensure List is Clear //
        CurrentPlayers.Clear();
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