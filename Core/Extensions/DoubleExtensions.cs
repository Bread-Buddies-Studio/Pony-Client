using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions;

public static partial class DoubleExtensions
{
    public static double Clamp(this double number, double min, double max) => Math.Clamp(number, min, max);
    public static double Clamp01(this double number) => number.Clamp(0, 1);
}