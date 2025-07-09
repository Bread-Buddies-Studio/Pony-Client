using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Rendering.Effects;

public abstract class Effect : IDisposable
{
    // Instance Data //
    public readonly Typewriter Parent;
    public required double EffectLevel;
    // Constructor //
    [SetsRequiredMembers]
    public Effect(Typewriter Parent, double EffectLevel)
    {
        // Init //
        this.Parent = Parent;
        this.EffectLevel = EffectLevel;
        // Add Reference //
        Parent.AddEffect(this);
    }
    // Matrix Methods //
    public abstract Matrix GetWordMatrix(int index, string word);
    public abstract Matrix GetLetterMatrix(int index, char character);
    // Dispose Methods //
    public virtual void Dispose()
    {
        // Remove Reference //
        Parent.RemoveEffect(this);
        // Call //
        GC.SuppressFinalize(this);
    }
}
