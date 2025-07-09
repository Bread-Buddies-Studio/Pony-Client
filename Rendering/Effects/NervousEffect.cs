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

namespace Rendering.Effects;

[method: SetsRequiredMembers]
public class NervousEffect(Typewriter Parent, double EffectLevel) : Effect(Parent, EffectLevel)
{
    // Overriden Methods //
    public override Matrix GetLetterMatrix(int index, char letter)
    {
        double xOffset = Random.Shared.NextDouble(-EffectLevel / 2, EffectLevel / 2);
        double yOffset = Random.Shared.NextDouble(-EffectLevel / 2, EffectLevel / 2);
        return new(1, 0, 0, 1, xOffset, yOffset);
    }

    public override Matrix GetWordMatrix(int index, string word)
    {
        return Matrix.Identity;
    }
}