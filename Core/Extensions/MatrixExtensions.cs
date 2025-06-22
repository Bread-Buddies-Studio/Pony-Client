using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Core.Extensions;

public static class MatrixExtensions
{
    public static Matrix Rotate(this Matrix matrix, double radians, Point center = new())
    {
        return matrix.Append(Matrix.CreateRotation(radians, center));
    }
}
