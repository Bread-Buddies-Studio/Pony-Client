using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Core.Geometry;

public class Line(Point P1, Point P2)
{
    public Point P1 = P1;
    public Point P2 = P2;

    // Base Methods //
    public bool Intersects(Line otherLine)
    {
        return false;
    }

    //public bool Intersects(Rect hitbox)
    //{
    //    hitbox.
    //}
    
    public static void Main()
    {
        Line line1 = new(
            new(-5, 0), 
            new(+5, 0)
        );

        Line line2 = new(
            new(0, +1),
            new(0, +5)
        );

        Debug.WriteLine(line1.Intersects(line2));
    }
}
