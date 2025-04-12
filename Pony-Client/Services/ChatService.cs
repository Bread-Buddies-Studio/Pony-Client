using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Pony_Client.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pony_Client.Extensions;
using Avalonia.Controls.Templates;

namespace Pony_Client.Services;

public class ChatEventArgs : EventArgs
{
    // Data //
    public required string Message;
    // Constructor //
    [SetsRequiredMembers]
    public ChatEventArgs(string Message) => this.Message = Message;
}

public static class ChatService
{
    private static List<string> _log = [];
    public static List<string> Log
    { 
        get => _log;

        private set => _log = value;
    }
    // Events //
    public static event EventHandler<ChatEventArgs>? Chatted;
    #region Private Methods
    
    #endregion
    // Base Methods //
    public static void AddLog(Entity speaker, string message)
    {
        Debug.WriteLine(speaker.Name);
        Log.Add($"{speaker.Name}: {message}");
        _ = new Speech(speaker, message);
        Chatted?.Invoke(speaker, new ChatEventArgs(message));
    }
}

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
    // Hitbox //
    public new Rect Hitbox => new(Position, Size);
    public new Point Position
    {
        get
        {
            // Extract Data //
            FormattedText formattedText = GetText();
            // Positions //
            Point basePosition = Speaker.Hitbox.Center - new Point(0, Speaker.Size.Height / 2);
            Point offset = GetOffset();
            // Offset //
            offset -= new Point(formattedText.Width / 2, formattedText.Height);
            // Return //
            return basePosition + offset;
        }
    }
    public new Size Size
    {
        get
        {
            // Extract Data //
            FormattedText formattedText = GetText();
            // Return //
            return new(formattedText.Width, formattedText.Height);
        }
    }
    #region Constructors
    public OverheadText(Entity Speaker, string Message) : base()
    {
        // Initialize //
        this.Speaker = Speaker;
        this.Message = Message;
        // Reference //
        OverheadTexts.Add(this);
    }
    #endregion
    #region Public Methods
    public FormattedText GetText()
    {
        Brush brush = new SolidColorBrush()
        {
            Opacity = this.Opacity,
            Color = Colors.White
        };

        FormattedText formattedText = new(Message, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface.Default, FontSize, brush)
        {
            TextAlignment = TextAlignment.Center,
        };

        return formattedText;
    }
    #endregion
    #region Private Methods
    private Point GetOffset()
    {
        Point offset = new();

        foreach (var overheadText in OverheadTexts.Reverse<OverheadText>())
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
    #region Override Methods
    public override void OnRender(GuiService guiService, DrawingContext context)
    {
        // Conditions //
        if (AttachedCamera is null)
            return;
        // Format Message //
        FormattedText formattedText = GetText();
        PushTransform(context, AttachedCamera.OffsetMatrix);
        // Draw //
        context.DrawText(formattedText, Position);
        PopMatrix();
    }
    public override void Delete()
    {
        Debug.WriteLine("Deleting!");
        Speaker.OverheadTexts.Remove(this);
        base.Delete();
    }
    #endregion
}

public class Speech : OverheadText
{
    // Coords //
    public new Rect Hitbox => new(Position, Size);
    public new Point Position => Speaker.Hitbox.Center - new Point(0, Speaker.Size.Height / 2);
    public new Size Size { 
        get 
        {
            var formattedText = GetText();
            return new(formattedText.Width, formattedText.Height);
        } 
    }
    public double FadeTime = 5_000;
    // Private Data //
    private long lifeTime = 0;
    // Constructor //
    [SetsRequiredMembers]
    public Speech(Entity Speaker, string Message) : base(Speaker, Message)
    {
        // Initialize //
        this.Speaker = Speaker;
        this.Message = Message;
        // Connections //
        RunService.Update += OnUpdate;
    }
    // Event Methods //
    public void OnUpdate(object? sender, UpdateEventArgs eventData)
    {
        // Extract Data //
        long deltaTime = eventData.DeltaTime;
        // Update Time //
        lifeTime += deltaTime;
        // Calculate //
        double alpha = (1 - lifeTime / FadeTime).Clamp01();
        // Update Opacity //
        Opacity = alpha;
        // Conditions //
        if (Opacity is 0)
        {
            Debug.WriteLine("Deleting!");
            Delete(); // Delete //
        }
    }
    //public override void OnRender(GuiService guiService, DrawingContext context)
    //{
    //    // Conditions //
    //    if (Speaker.AttachedCamera is null)
    //        return;
    //    // Get Text //
    //    FormattedText formattedText = GetText();
    //    // Positions //
    //    Point offset = new(-formattedText.Width / 2, -formattedText.Height);
    //    // Get Height //
    //    foreach (Speech otherSpeech in Speaker.Speeches.Reverse<Speech>())
    //    {
    //        // Data //
    //        FormattedText otherText = otherSpeech.GetText();
    //        double otherHeight = otherText.Height;
    //        // Settings //
    //        offset = offset.WithY(offset.Y - otherHeight);
    //        // Conditions //
    //        if (this == otherSpeech)
    //            break;
    //    }
    //    // Checks //
    //    using (context.PushTransform(Speaker.AttachedCamera.OffsetMatrix))
    //    {
    //        context.DrawText(formattedText, Position + offset);
    //    }
    //}
    // Inherited Methods //
    public override void Delete()
    {
        base.Delete();
        // Disconnect //
        RunService.Update -= OnUpdate;
    }
}
