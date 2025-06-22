// Directory //
namespace Rendering.Entities.Tools;
// Imports //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Core.Interfaces;
using Core.Services;
using Rendering.Views;
using Rendering.Visible;

// Class //
public abstract class Projectile : Renderable, ICollidable
{
    // Arrays //
    public Dictionary<Player, long> HitList { get; } = [];
    // Data //
    public int HitCooldown { get; set; } = 0;
    public int Piercing { get; set; } = 0;
    public int Damage { get; set; } = 10;
    public bool MultiHit { get; set; }
    public bool IsCollidable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsAnchored { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    // Constructor //
    #region Collision Methods
    public IEnumerable<Player> GetTouching() => Player.Players.Where(player =>
    {
        // Conditions //
        if (!IsCollidingWith(player))
            return false;
        if (HitList.ContainsKey(player))
            return false;
        // Success //
        return true;
    });
    public bool IsCollidingWith(Rect Hitbox) => this.Hitbox.Intersects(Hitbox);
    public bool IsCollidingWith(ICollidable other) => IsCollidingWith(other.Hitbox);
    #endregion
    // Event Methods //
    private void OnUpdate(object? sender, UpdateEventArgs eventData)
    {
        // Coll Detection //
        IEnumerable<Player> touchingPlayers = GetTouching();
        // Hit Cooldown Remover //
        foreach ((Player player, long timeStamp) in HitList)
        {
            // Conditions //
            if (RunService.CurrentMillis - timeStamp < HitCooldown)
                continue;
            // Success //
            HitList.Remove(player);
        }
        // Damage Players //
        foreach (var player in touchingPlayers)
        {
            // Activate Damage //
            player.Health -= Damage;
            // Add Hit Cooldown //
            HitList.Add(player, RunService.CurrentMillis);
        }
    }
    public override void OnRender(GuiService guiService, DrawingContext context)
    {
    }
    // Deletion Methods //
    public override void Delete()
    {
        base.Delete();
    }
}