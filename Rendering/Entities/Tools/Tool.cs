// Directory //
namespace Rendering.Entities.Tools;

// Imports //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rendering.Visible;
// Classes //
public abstract class Tool : Renderable
{
    // Static Data //
    public static readonly HashSet<Tool> Tools = [];
    // Instance Data //
    public required string Name;
    public required string Description;
    public required string Icon;

    public Entity? Parent;
    // Constructor //
    public Tool()
    {
    }
    // Methods //
    public abstract void Use();
}