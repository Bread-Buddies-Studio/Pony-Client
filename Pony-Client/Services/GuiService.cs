using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Pony_Client.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace Pony_Client.Services;
// Classes //
public abstract class Renderable : IComparable<Renderable>
{
    // Static Data //
    public static readonly SortedDictionary<Renderable, byte> Renderables = [];
    private readonly List<DrawingContext.PushedState> Matrixes = [];
    // Constructor //
    public Renderable() : base()
    {
        Renderables[this] = Layer;
    }
    public Renderable(Rect Hitbox) : this()
    {
        // Initialize //
        this.Hitbox = Hitbox;
    }
    // Identifier //
    public readonly Guid UniqueIdentifier = Guid.NewGuid();
    // Hitbox //
    public Rect Hitbox { get; set; }
    public Point Position
    { 
        get => Hitbox.Position;
        set => Hitbox = new(value, Size);
    }
    public Size Size 
    { 
        get => Hitbox.Size;
        set => Hitbox = new(Position, value);
    }
    // Render Data //
    private byte _layer;
    public byte Layer 
    {
        get => _layer;

        set
        {
            Renderables.Remove(this);
            // Settings //
            _layer = value;
            Renderables[this] = value;
        }
    }
    /// <summary> Position Based Layering </summary>
    public bool PBL = false;
    // Base Methods //
    public void PushTransform(DrawingContext context, Matrix transform)
    {
        Matrixes.Insert(0, context.PushTransform(transform));
    }
    public void PopMatrix()
    {
        foreach (var matrix in Matrixes)
            matrix.Dispose();

        Matrixes.Clear();
    }
    // Event Methods //
    public abstract void OnRender(GuiService guiService, DrawingContext context);
    // Deletion Methods //
    public virtual void Delete() => Renderables.Remove(this);
    // Ancestor Methods //
    public int CompareTo(Renderable? renderable)
    {
        // Conditions //
        if (renderable is null)
            return -1;
        if (this.UniqueIdentifier == renderable.UniqueIdentifier)
            return 0;
        if (renderable.Layer == Layer)
            return -1;
        // Return //
        return renderable.Layer > Layer ? 1 : -1;
    }
}
// Service //
public partial class GuiService : UserControl
{
    public static GuiService? Window;
    private LocalPlayer? localPlayer => WorldService.localPlayer;
    // Constructor //
    public GuiService()
    {
        // Initialize //
        InitializeComponent();
        // Connections //
        RunService.Update += OnUpdate;
        ChatService.Chatted += OnChatted;
        InputService.KeyDown += OnKeyDown;
        // Reference //
        Window = this;
    }
    // Static Methods //
    public static Bitmap LoadFromResource(Uri resourceUri)
    {
        return new Bitmap(AssetLoader.Open(resourceUri));
    }
    public static async Task<Bitmap?> LoadFromWeb(Uri url)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            return new Bitmap(new MemoryStream(data));
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
            return null;
        }
    }
    // Event Methods //
    public async void OnKeyDown(object? sender, KeyEventArgs eventData)
    {
        // Conditions //
        if (localPlayer is null)
            return;
        // Checks //
        switch(eventData.Key)
        {
            case Key.Enter:
                // Conditions //
                if (!ChatInput.IsFocused)
                    break;
                if (string.IsNullOrEmpty(ChatInput.Text))
                    break;
                // Success //
                localPlayer.Speak(ChatInput.Text);
                ChatInput.Clear();
                break;
            case Key.OemQuestion:
                ChatInput.Focus();
                await RunService.UntilNextUpdate();
                ChatInput.Clear();
                break;
            case Key.Escape:
                TopLevel.GetTopLevel(this)?.FocusManager?.ClearFocus();
                break;
            default:
                break;
        }
    }
    public void OnChatted(object? _speaker, ChatEventArgs eventData)
    {
        // Get Text //
        string Log = string.Empty;

        foreach (string message in ChatService.Log)
            Log += $"{message}\n";
        // Show Text //
        ChatLog.Text = Log;
        TopLevel.GetTopLevel(this)?.FocusManager?.ClearFocus();
    }
    public void OnUpdate(object? _, UpdateEventArgs eventData)
    {
        // Checks //
        if (VisualRoot is not null)
        {   // Update Size //
            Width = VisualRoot.ClientSize.Width;
            Height = VisualRoot.ClientSize.Height;
        }
    }
    public override void Render(DrawingContext context)
    {
        // Conditions //
        base.Render(context);
        // Invoke //
        RunService.InvokeRender(this, context);
        // Invoke Events //
        foreach (Renderable renderable in Renderable.Renderables.Keys)
        {
            renderable.OnRender(this, context);
        }
    }
}
