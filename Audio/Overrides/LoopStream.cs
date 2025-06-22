using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Audio.Overrides;

public class LoopStream(WaveStream _sourceStream, long LoopBegin = 0, long? LoopEnd = null) : WaveStream
{
    // Private Data //
    private readonly WaveStream _sourceStream = _sourceStream;
    // Public Data //
    public long LoopBegin = LoopBegin;
    public long LoopEnd = LoopEnd ?? _sourceStream.Length;
    public bool EnableLooping { get; set; } = true;

    public override WaveFormat WaveFormat => _sourceStream.WaveFormat;

    public override long Length => _sourceStream.Length;

    public override long Position 
    { 
        get => _sourceStream.Position;
        set => _sourceStream.Position = value; 
    }
    // Override Methods //
    public override int Read(byte[] buffer, int offset, int count)
    {
        // Cache //
        int totalBytesRead = 0;
        TimeSpan LoopBegin = TimeSpan.FromMilliseconds(this.LoopBegin);
        TimeSpan LoopEnd = TimeSpan.FromMilliseconds(this.LoopEnd);
        // Read //
        while (totalBytesRead < count)
        {
            int bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
            // Update //
            totalBytesRead += bytesRead;
            // Conditions //
            if (!EnableLooping)
                break;
            // Checks //
            if (_sourceStream.CurrentTime >= LoopEnd)
                _sourceStream.CurrentTime = LoopBegin;
            if (_sourceStream.Position is 0 || _sourceStream.Position == Length)
                _sourceStream.CurrentTime = LoopBegin;
        }
        // Finished Reading //
        return totalBytesRead;
    }
}
