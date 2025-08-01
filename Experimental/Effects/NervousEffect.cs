using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Core.Extensions;
using Rendering.Views;
using Rendering.Visible;

namespace Experimental.Effects;

[method: SetsRequiredMembers]
public class NervousEffect(Typewriter Parent, double EffectLevel) : Effect(Parent, EffectLevel)
{
    //

    // Overriden Methods //
    public override DrawingContext.PushedState[] GetLetterMatrix(DrawingContext context, int index, char letter)
    {
        // Calculations //
        double xOffset = Random.Shared.NextDouble(-EffectLevel / 2, EffectLevel / 2);
        double yOffset = Random.Shared.NextDouble(-EffectLevel / 2, EffectLevel / 2);

        return [context.PushTransform(Matrix.CreateTranslation(xOffset, yOffset))];
    }

    public override DrawingContext.PushedState[]? GetWordMatrix(DrawingContext context, int index, string word)
    {
        return null;
    }
}