using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Core.Extensions;

namespace Core.Interfaces;

public interface ICollidable : IPhysical
{
    public bool IsAnchored { get; set; }
    public bool IsCollidable { get; set; }
    public bool IsCollidingWith(Rect otherHitbox) => IsCollidable && Hitbox.Intersects(otherHitbox);
    public bool IsCollidingWith(ICollidable other) => other.IsCollidable && IsCollidingWith(other.Hitbox);
    public void Seperate(ICollidable other)
    {
        // Conditions //
        if (!IsCollidingWith(other))
            return;
        // Calculate Difference //
        Rect difference = Hitbox.Intersect(other.Hitbox);
        double anchoredDivisor = IsAnchored ? 1 : 2;

        if (difference.Width < difference.Height)
        {
            Position = Position.TranslateX(
                Hitbox.X > other.Hitbox.X ? difference.Width / anchoredDivisor : -difference.Width / anchoredDivisor
            );

            if (!other.IsAnchored)
            {
                other.Position = other.Position.TranslateX(
                    other.Hitbox.X > Hitbox.X ? difference.Width / anchoredDivisor : -difference.Width / anchoredDivisor // If Other Past this on X-Axis //
                );
            }
        }

        else
        {
            Position = Position.TranslateY(
                Hitbox.Y > other.Hitbox.Y ? difference.Height / anchoredDivisor : -difference.Height / anchoredDivisor
            );

            if (!other.IsAnchored)
            {
                other.Position = other.Position.TranslateY(
                    other.Hitbox.Y > Hitbox.Y ? difference.Height / anchoredDivisor : -difference.Height / anchoredDivisor // If Other Past this on Y-Axis //
                );
            }
        }
    }
}