using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using Core.Extensions;

namespace Rendering.Effects;

public enum FlickerType
{
    Word,
    Letter
}

[method: SetsRequiredMembers]
public class FlickerEffect(Typewriter Parent) : Effect(Parent)
{
    public override DrawingContext.PushedState[]? GetLetterMatrix(DrawingContext context, int index, char character, string? internalString)
    {
        // Pre-Conditions //
        if (internalString is null)
            return null;
        // Split Data //
        if (!double.TryParse(internalString, out double EffectLevel))
            Debug.WriteLine("Invalid Internal String: " + internalString);
        // Calculations //
        double opacity = Random.Shared.NextDouble(1 - EffectLevel, 1);
        // Return Opacity //
        return [Parent.PushOpacity(context, opacity)];
    }

    public override DrawingContext.PushedState[]? GetWordMatrix(DrawingContext context, int index, string word, string? internalString) => null;
}
