using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Rendering.Effects;

[method: SetsRequiredMembers]
public class SineEffect(Typewriter Parent, double Speed, double EffectLevel) : Effect(Parent, EffectLevel)
{
    public required double Speed = Speed;
    public override Matrix GetLetterMatrix(int index, char character)
    {
        return new (1, 0, 0, 1, 0, Math.Sin(index + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d * Speed) * EffectLevel);
    }

    public override Matrix GetWordMatrix(int index, string word)
    {
        return Matrix.Identity;
    }
}
