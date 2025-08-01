using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Core.Extensions;
using Rendering.Effects;

namespace Rendering.Effects;

public abstract class Effect : IDisposable
{
    // Instance Data //
    public readonly Typewriter Parent;
    // Constructor //
    [SetsRequiredMembers]
    public Effect(Typewriter Parent)
    {
        // Init //
        this.Parent = Parent;
        // Add Reference //
        Parent.AddEffect(this);
    }
    // Matrix Methods //
    public DrawingContext.PushedState[]? WordMatrix(DrawingContext context, int index, string word)
    {
        // Data //
        string effectName = this.GetType().Name
            .ToLower()
            .Replace("effect", string.Empty);
        // Conditions //
        if (!Parent.BBCodeMatches.IsIndexWithin(index, effectName))
            return null;
        // Call //
        return GetWordMatrix(context, index, word, Parent.BBCodeMatches.FindClosestPreviousStartingMatch(index, effectName)?.GetInternalString());
    }    
    public DrawingContext.PushedState[]? LetterMatrix(DrawingContext context, int index, char word)
    {
        // Data //
        string effectName = this.GetType().Name
            .ToLower()
            .Replace("effect", string.Empty);
        // Conditions //
        if (!Parent.BBCodeMatches.IsIndexWithin(index, effectName))
            return null;
        // Call //
        return GetLetterMatrix(context, index, word, Parent.BBCodeMatches.FindClosestPreviousStartingMatch(index, effectName)?.GetInternalString());
    }
    public abstract DrawingContext.PushedState[]? GetWordMatrix(DrawingContext context, int index, string word, string? internalString);
    public abstract DrawingContext.PushedState[]? GetLetterMatrix(DrawingContext context, int index, char character, string? internalString);
    // Dispose Methods //
    public virtual void Dispose()
    {
        // Remove Reference //
        Parent.RemoveEffect(this);
        // Call //
        GC.SuppressFinalize(this);
    }
}
