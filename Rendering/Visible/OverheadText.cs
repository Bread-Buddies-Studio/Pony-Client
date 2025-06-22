using Avalonia.Media;
using Avalonia;
using Rendering.Entities;
using Rendering.Views;
using Rendering.Visible;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Core.Services;
using System.Diagnostics.CodeAnalysis;
using Tmds.DBus.Protocol;

namespace Rendering.Visible;

public abstract class OverheadText : Renderable
{
    // Lists //
    public List<OverheadText> OverheadTexts => Speaker.OverheadTexts;
    public Camera? AttachedCamera => Speaker.AttachedCamera;
    // Required Data //
    public required Entity Speaker;
    public required string Message;
    // Text Settings //
    public required double Opacity = 1;
    public required double FontSize = 20;
    // Properties //
    private SolidColorBrush Brush
    {
        get
        {
            // Update //
            field.Opacity = Opacity;
            field.Color = Colors.White;
            // Give //
            return field;
        }
    } = new();
    public FormattedText FormattedText => new(Message, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface.Default, FontSize, Brush)
    {
        TextAlignment = TextAlignment.Center 
    };
    #region Constructors
    [SetsRequiredMembers]
    public OverheadText(Entity Speaker, string Message) : base()
    {
        // Initialize //
        this.Speaker = Speaker;
        this.Message = Message;
        // Reference //
        OverheadTexts.Add(this);
        // Connections //
        RunService.Update += OnUpdate;
    }
    #endregion
    #region Public Methods
    #endregion
    #region Private Methods
    public Point GetOffset()
    {
        Point offset = new();

        foreach (var overheadText in OverheadTexts)
        {
            // Conditions //
            if (this == overheadText)
                break;
            // Settings //
            offset -= new Point(0, overheadText.Size.Height);
        }

        return offset;
    }
    #endregion
    #region Event Methods
    public virtual void OnUpdate(object? sender, UpdateEventArgs eventData)
    {
        // Positions //
        Point basePosition = Speaker.Hitbox.Center - new Point(0, Speaker.Size.Height / 2);
        Point offset = GetOffset();
        // Offset //
        offset -= new Point(FormattedText.Width / 2, 0);
        // Return //
        Position = basePosition + offset;
        Size = new(FormattedText.Width, FormattedText.Height);
    }

    public override void OnRender(GuiService guiService, DrawingContext context)
    {
        // Conditions //
        if (AttachedCamera is null)
            return;
        // Format Message //
        PushTransform(context, AttachedCamera.OffsetMatrix);
        // Draw //
        context.DrawText(FormattedText, Position);
        PopMatrix();
    }
    #endregion
    // Disposal Methods //
    public override void Delete()
    {
        // Delete //
        Debug.WriteLine("Deleting!");
        // Remove Reference //
        Speaker.OverheadTexts.Remove(this);
        // Disconnections //
        RunService.Update -= OnUpdate;
        base.Delete();
    }
}