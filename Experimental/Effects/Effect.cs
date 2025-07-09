using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Experimental.Effects;

namespace Experimental.Effects;

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
    public abstract DrawingContext.PushedState[]? GetWordMatrix(DrawingContext context, int index, string word);
    public abstract DrawingContext.PushedState[]? GetLetterMatrix(DrawingContext context, int index, char character);
    // Dispose Methods //
    public virtual void Dispose()
    {
        // Remove Reference //
        Parent.RemoveEffect(this);
        // Call //
        GC.SuppressFinalize(this);
    }
}
