using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Core.Interfaces;

public interface IPhysical
{
    public abstract Rect Hitbox { get; set; }
    public virtual Point Position
    {
        get => Hitbox.Position;
        set => Hitbox = new(value, Size);
    }
    public virtual Size Size
    {
        get => Hitbox.Size;
        set => Hitbox = new(Position, value);
    }

    public void Translate(Point Offset) => Position += Offset;
}
