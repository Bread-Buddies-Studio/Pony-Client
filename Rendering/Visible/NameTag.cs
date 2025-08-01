using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using Core.Services;
using Rendering.Entities;

namespace Rendering.Visible;

public class NameTag : OverheadText
{
    public override bool IsAlwaysOnTop => true;
    [SetsRequiredMembers]
    public NameTag(Entity Speaker) : base(Speaker, Speaker.Name) => RunService.Update += OnUpdate;
    // Event Methods //
    public override void OnUpdate(object? sender, UpdateEventArgs eventData)
    {
        Message = Speaker.Name;
        base.OnUpdate(sender, eventData);
    }
}
