//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Core.Services;
//using NAudio.Wave;
//using NAudio.Wave.SampleProviders;

//namespace Audio.Structs;

//public struct Music
//{
//    // Instance Data //
//    public required string Name { get; init; }
//    public required long Duration { get; init; }
//    // Loop Data //

//    public required bool Looped = false;
//    public required long LoopBegin;
//    public required long LoopEnd;
//    // Media Data //
//    public required WaveFileReader Media
//    {
//        readonly get => field;
//        set
//        {
//            OffsetProvider = new(value)
//            {
//                SkipOver = TimeSpan.FromMilliseconds(LoopBegin),
//                Take = TimeSpan.FromMilliseconds(LoopEnd)
//            };

//            field = value;
//        }
//    }

//    [field: AllowNull]
//    public OffsetSampleProvider OffsetProvider
//    { 
//        get
//        {
//            field ??= new(Media.Sk)
//                {
//                    SkipOver = TimeSpan.FromMilliseconds(LoopBegin),
//                    Take = TimeSpan.FromMilliseconds(Duration - LoopEnd)
//                };

//            return field;
//        }

//        set => field = value;
//    }
//    // Player Data //
//    public required WaveOutEvent MediaPlayer { get; init; }
//    public readonly float Volume
//    {
//        get => MediaPlayer.Volume;
//        set => MediaPlayer.Volume = value;
//    }

//    public readonly long CurrentTime
//    {
//        get => Media.CurrentTime.Milliseconds;
//        set => Media.CurrentTime = TimeSpan.FromMilliseconds(value);
//    }
//    // Constructor //
//    [SetsRequiredMembers]
//    public Music(string Name, long LoopBegin = 0, long? LoopEnd = null)
//    {
//        // Initialize //
//        this.Name = Name;
//        // Data Source //
//        Media = AudioService.GetMedia(Name);
//        // Extract Data //
//        Duration = Media.Length;
//        // Loop Settings //
//        this.LoopBegin = LoopBegin;
//        this.LoopEnd = LoopEnd ?? Duration;
//        // Initialize Offset //
//        OffsetProvider = new(Media)
//        {
//            SkipOver = TimeSpan.FromMilliseconds(this.LoopBegin),
//            Take = TimeSpan.FromMilliseconds(Duration - this.LoopEnd)
//        };
//        // Initialize Media Player //
//        MediaPlayer = new();
//        MediaPlayer.Init(OffsetProvider);
//        MediaPlayer.Play();

//        Debug.WriteLine(LoopBegin);
//        Debug.WriteLine($"Intended Skip Over: {TimeSpan.FromMilliseconds(this.LoopBegin)}");
//        Debug.WriteLine($"Skip Over: {OffsetProvider?.SkipOver}");
//        Debug.WriteLine($"Take: {OffsetProvider?.Take}");
//        // Connections //
//        //MediaPlayer.PlaybackStopped += OnPlaybackStopped;
//    }

//    //private void OnPlaybackStopped(object? sender, StoppedEventArgs eventData)
//    //{
//    //    // Conditions //
//    //    if (!Looped)
//    //        return;
//    //    // Reset Position //
//    //    CurrentTime = LoopBegin;
//    //    // Play Again //
//    //    Debug.WriteLine("Stopped!");
//    //    MediaPlayer.Play();
//    //}
//}
