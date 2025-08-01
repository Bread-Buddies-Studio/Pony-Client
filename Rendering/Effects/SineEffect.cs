using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Core.Extensions;
using Core.Services;

namespace Rendering.Effects;

[method: SetsRequiredMembers]
public class SineEffect(Typewriter Parent) : Effect(Parent)
{
    public override DrawingContext.PushedState[]? GetLetterMatrix(DrawingContext context, int index, char character, string? internalString)
    {
        // Conditions //
        if (internalString is null)
            return null;
        // Split Data //
        string[] internalStrings = internalString.Split(' ');
        // Checks //
        if (!double.TryParse(internalStrings[0], out double EffectLevel))
            Debug.WriteLine("Invalid Internal String: " + internalString);
        if (!double.TryParse(internalStrings[1], out double Speed))
            Debug.WriteLine("Invalid Internal String: " + internalString);
        // Calculations //
        double xOffset = 0;
        double yOffset = Math.Sin(index + RunService.CurrentMillis / 1000d * Speed) * EffectLevel;
        // Return //
        return
        [
            Parent.PushTransform(context, Matrix.CreateTranslation(xOffset, yOffset))
        ];
    }

    public override DrawingContext.PushedState[]? GetWordMatrix(DrawingContext context, int index, string word, string? internalString)
    {
        return null;
    }
}
