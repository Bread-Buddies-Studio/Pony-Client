using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Core.Extensions;
using Rendering.Effects;
using Rendering.Views;
using Rendering.Visible;

namespace Rendering.Effects;

[method: SetsRequiredMembers]
public class NervousEffect(Typewriter Parent) : Effect(Parent)
{
    // Overriden Methods //
    public override DrawingContext.PushedState[] GetLetterMatrix(DrawingContext context, int index, char letter, string? internalString)
    {
        // Conditions //
        if (!double.TryParse(internalString, out double EffectLevel))
            Debug.WriteLine("Invalid Internal String: " + internalString);
        // Calculations //
        double xOffset = Random.Shared.NextDouble(-EffectLevel / 2, EffectLevel / 2);
        double yOffset = Random.Shared.NextDouble(-EffectLevel / 2, EffectLevel / 2);
        // Debug //
        //Debug.WriteLine($"Offset: {{{xOffset}, {yOffset}}}");
        // Success //
        return
        [
            Parent.PushTransform(context, Matrix.CreateTranslation(xOffset, yOffset)),
        ];
    }

    public override DrawingContext.PushedState[]? GetWordMatrix(DrawingContext context, int index, string word, string? internalString)
    {
        return null;
    }
}