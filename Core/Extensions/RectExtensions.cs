using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Core.Extensions;

public static class RectExtensions
{
    public static void Seperate(this Rect rect1, Rect rect2)
    {
        // Conditions //
        if (!rect1.Intersects(rect2))
            return;
        // Calculate //
        Rect intersection = rect1.Intersect(rect2);
        double xDifference = intersection.Width;
        double yDifference = intersection.Height;
        // Move Hitboxes //
        rect1.Translate(new(
            rect1.X > rect2.X ? xDifference / 2 : -xDifference / 2,
            rect1.Y > rect2.Y ? yDifference / 2 : -yDifference / 2
        ));

        rect1.Translate(new(
            rect2.X > rect1.X ? xDifference / 2 : -xDifference / 2,
            rect2.Y > rect1.Y ? yDifference / 2 : -yDifference / 2
        ));
    }
}
