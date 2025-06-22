using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Audio.Services;
using NAudio.Wave;

namespace Audio.Overrides;

/// <inheritdoc cref="WaveOutEvent"/>/>
public class MediaPlayer : WaveOutEvent, IDisposable
{
    // Public Data //
    /// <summary>
    /// Name of the media file being played by this instance.
    /// </summary>
    public readonly string Name;
    public bool IsLooped { get; set; } = false;
    // Constructors //
    /// <inheritdoc cref="WaveOutEvent()"/>
    /// <param name="Name">Name of the media file</param>
    [SetsRequiredMembers]
    public MediaPlayer(string Name) : base()
    {
        // Initialize //
        this.Name = Name;
        // Add Reference //
        AudioService.CurrentPlayers.Add(this);
    }
    // Base Methods //
    /// <inheritdoc cref="WaveOutEvent.Play"/>
    /// <returns>This intance.</returns>
    public new MediaPlayer Play()
    {
        base.Play();
        return this;
    }
    // Deletion Methods //
    public new void Dispose()
    {
        // Delete Reference //
        AudioService.CurrentPlayers.Remove(this);
        // Dispose of the base class //
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}
